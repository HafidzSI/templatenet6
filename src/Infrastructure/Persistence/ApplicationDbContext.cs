// -----------------------------------------------------------------------------------
// ApplicationDbContext.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Extensions;
using NetCa.Infrastructure.Persistence.Interceptors;
using Newtonsoft.Json;

namespace NetCa.Infrastructure.Persistence;

/// <summary>
/// ApplicationDbContext
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IUserAuthorizationService _userAuthorizationService;
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IDateTime _dateTime;
    private readonly AppSetting _appSetting;
    private readonly List<EntityState> _auditState = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userAuthorizationService"></param>
    /// <param name="mediator"></param>
    /// <param name="auditableEntitySaveChangesInterceptor"></param>
    /// <param name="dateTime"></param>
    /// <param name="appSetting"></param>
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IUserAuthorizationService userAuthorizationService,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        IDateTime dateTime,
        AppSetting appSetting)
        : base(options)
    {
        _userAuthorizationService = userAuthorizationService;
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _dateTime = dateTime;
        _appSetting = appSetting;

        foreach (var state in _appSetting.DatabaseSettings.AuditState)
            _auditState.Add((EntityState)Enum.Parse(typeof(EntityState), state));
    }

    /// <summary>
    /// Gets or sets Changelogs
    /// </summary>
    public DbSet<Changelog> Changelogs { get; set; }

    /// <summary>
    /// Gets or sets KafkaMessage
    /// </summary>
    public DbSet<MessageBroker> MessageBroker { get; set; }

    /// <summary>
    /// Gets or sets ReceivedMessageBroker
    /// </summary>
    public DbSet<ReceivedMessageBroker> ReceivedMessageBroker { get; set; }

    /// <summary>
    /// AsNoTracking
    /// </summary>
    public void AsNoTracking()
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear()
    {
        ChangeTracker.Clear();
    }

    /// <summary>
    /// to prevent hard delete
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Exception</exception>
    public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// to prevent hard delete
    /// </summary>
    /// <param name="entities"></param>
    /// <exception cref="NotImplementedException">Exception</exception>
    public override void RemoveRange(params object[] entities)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// to prevent hard delete
    /// </summary>
    /// <param name="entities"></param>
    /// <exception cref="NotImplementedException">Exception</exception>
    public override void RemoveRange(IEnumerable<object> entities)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// to prevent hard delete
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DepedencyInjection).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// OnConfiguring
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    /// <summary>
    /// Execute using EF Core resiliency strategy
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExecuteResiliencyAsync(Func<Task> action)
    {
        var strategy = this.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await this.Database.BeginTransactionAsync();
            await action();
            await transaction.CommitAsync();
        });
    }

    /// <summary>
    /// SaveChangesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Exception</exception>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);
        var changelogEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(changelogEntries, cancellationToken);
        return result;
    }

    private List<ChangelogEntry> OnBeforeSaveChanges()
    {
        var ignoreTable = new List<string>
        {
            nameof(Changelog),
            nameof(MessageBroker),
            nameof(ReceivedMessageBroker)
        };

        ChangeTracker.DetectChanges();
        var changelogEntries = new List<ChangelogEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Changelog || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var tableName = entry.Metadata.GetTableName();

            var enableAuditChangelog = _appSetting.DatabaseSettings.EnableAuditChangelog &&
                !ignoreTable.Contains(tableName) &&
                _auditState.Contains(entry.State);

            if (enableAuditChangelog)
            {
                var username = _userAuthorizationService.GetUserNameSystem();

                var changelogEntry = new ChangelogEntry
                {
                    ChangeBy = username,
                    ChangeDate = _dateTime.UtcNow,
                    TableName = tableName
                };

                changelogEntries.Add(changelogEntry);

                ChangelogEntryEvent(changelogEntry, entry);
            }
        }

        foreach (var changelogEntry in changelogEntries.Where(_ => !_.HasTemporaryProperties))
            Changelogs.Add(changelogEntry.ToAudit());

        return changelogEntries
            .Where(_ => _.HasTemporaryProperties)
            .ToList();
    }

    private void ReplaceUnicodePostgres(PropertyEntry property)
    {
        var con = Database.ProviderName;

        if (!con!.Equals("Npgsql.EntityFrameworkCore.PostgreSQL"))
            return;

        var type = property.Metadata.ClrType;
        var typeName = type.ShortDisplayName();

        if (typeName.Equals("string") && property.CurrentValue != null)
        {
            property.CurrentValue = Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()),
                    Encoding.UTF8.GetBytes(property.CurrentValue.ToString() !)));

            property.CurrentValue = ((string)property.CurrentValue)
                    .Replace("\u0000", string.Empty);
        }

        if (typeName.Equals("string") && property.OriginalValue != null)
        {
            property.OriginalValue = Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()),
                    Encoding.UTF8.GetBytes(property.OriginalValue.ToString() !)));

            property.OriginalValue = ((string)property.OriginalValue)
                .Replace("\u0000", string.Empty);
        }
    }

    private void ChangelogEntryEvent(ChangelogEntry changelogEntry, EntityEntry entry)
    {
        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary)
            {
                changelogEntry.TemporaryProperties.Add(property);
                continue;
            }

            var propertyName = property.Metadata.Name;

            if (property.Metadata.IsPrimaryKey())
            {
                changelogEntry.KeyValues[propertyName] = property.CurrentValue;
                continue;
            }

            ReplaceUnicodePostgres(property);

            switch (entry.State)
            {
                case EntityState.Added:
                    changelogEntry.NewValues[propertyName] = property.CurrentValue;
                    changelogEntry.Method = "ADD";
                    break;

                case EntityState.Deleted:
                    changelogEntry.OldValues[propertyName] = property.OriginalValue;
                    changelogEntry.Method = "DELETE";
                    break;

                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        changelogEntry.OldValues[propertyName] = property.OriginalValue;
                        changelogEntry.NewValues[propertyName] = property.CurrentValue;
                        changelogEntry.Method = "EDIT";
                    }

                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(entry.State.ToString(), "Un know entry.State.");
            }
        }
    }

    private Task OnAfterSaveChanges(List<ChangelogEntry> changelogEntries, CancellationToken cancellationToken)
    {
        if (changelogEntries == null || changelogEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var changelogEntry in changelogEntries)
        {
            foreach (var prop in changelogEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    changelogEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                else
                    changelogEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            Changelogs.Add(changelogEntry.ToAudit());
        }

        return SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// ChangelogEntry
/// </summary>
public class ChangelogEntry
{
    /// <summary>
    /// Gets or sets method
    /// </summary>
    /// <value></value>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets tableName
    /// </summary>
    /// <value></value>
    public string TableName { get; set; }

    /// <summary>
    /// Gets or sets changeBy
    /// </summary>
    /// <value></value>
    public string ChangeBy { get; set; }

    /// <summary>
    /// Gets or sets changeDate
    /// </summary>
    /// <value></value>
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// Gets keyValues
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> KeyValues { get; } = new ();

    /// <summary>
    /// Gets oldValues
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> OldValues { get; } = new ();

    /// <summary>
    /// Gets newValues
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> NewValues { get; } = new ();

    /// <summary>
    /// Gets temporaryProperties
    /// </summary>
    /// <returns></returns>
    public List<PropertyEntry> TemporaryProperties { get; } = new ();

    /// <summary>
    /// Gets a value indicating whether hasTemporaryProperties
    /// </summary>
    /// <returns></returns>
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    /// <summary>
    /// ToAudit save record for audit changelog
    /// </summary>
    /// <returns></returns>
    public Changelog ToAudit()
    {
        var changelog = new Changelog
        {
            TableName = TableName,
            ChangeBy = ChangeBy,
            ChangeDate = ChangeDate,
            Method = Method,
            KeyValues = JsonConvert.SerializeObject(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues)
        };

        return changelog;
    }
}

// -----------------------------------------------------------------------------------
// AuditableEntitySaveChangesInterceptor.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Persistence.Interceptors;

/// <summary>
/// AuditableEntitySaveChangesInterceptor
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IUserAuthorizationService _currentUserService;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntitySaveChangesInterceptor"/> class.
    /// </summary>
    /// <param name="currentUserService"></param>
    /// <param name="dateTime"></param>
    public AuditableEntitySaveChangesInterceptor(
        IUserAuthorizationService currentUserService,
        IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    /// <summary>
    /// SavingChanges
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// SavingChangesAsync
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="result"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// UpdateEntities
    /// </summary>
    /// <param name="context"></param>
    private void UpdateEntities(DbContext context)
    {
        if (context == null)
            return;

        var username = _currentUserService.GetUserNameSystem();

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
                    entity.CreatedBy ??= username;
                    entity.CreatedDate ??= _dateTime.UtcNow;
                    entity.IsActive ??= true;
                    break;
                case EntityState.Modified:
                    entity.UpdatedBy = username;
                    entity.UpdatedDate = _dateTime.UtcNow;
                    break;
            }
        }
    }
}

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
    /// <summary>
    /// HasChangedOwnedEntities
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}
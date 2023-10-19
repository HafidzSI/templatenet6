// -----------------------------------------------------------------------------------
// IApplicationDbContext.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetCa.Domain.Entities;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IApplicationDbContext
/// </summary>
public interface IApplicationDbContext
{
#pragma warning disable
    public DbSet<Changelog> Changelogs { get; set; }
    public DbSet<MessageBroker> MessageBroker { get; set; }
    public DbSet<ReceivedMessageBroker> ReceivedMessageBroker { get; set; }
#pragma warning restore

    /// <summary>
    /// Gets database
    /// </summary>
    public DatabaseFacade Database { get; }

    /// <summary>
    /// AsNoTracking
    /// </summary>
    public void AsNoTracking();

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear();

    /// <summary>
    /// Execute using EF Core resiliency strategy
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Task ExecuteResiliencyAsync(Func<Task> action);

    /// <summary>
    /// SaveChangesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
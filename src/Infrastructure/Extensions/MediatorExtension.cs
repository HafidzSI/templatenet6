// -----------------------------------------------------------------------------------
// MediatorExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Extensions;

/// <summary>
/// MediatorExtension
/// </summary>
public static class MediatorExtension
{
    /// <summary>
    /// DispatchDomainEvents
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var baseEntities = entities.ToList();
        var domainEvents = baseEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        baseEntities
            .ToList()
            .ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
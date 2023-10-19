// -----------------------------------------------------------------------------------
// BaseEntity.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCa.Domain.Common;

/// <summary>
/// BaseEntity
/// </summary>
public record BaseEntity
{
    /// <summary>
    /// Gets or sets id
    /// </summary>
    /// <value></value>
    public Guid Id { get; set; }

    private readonly List<BaseEvent> _domainEvents = new ();

    /// <summary>
    /// Gets DomainEvents
    /// </summary>
    /// <value></value>
    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// GetUserListAsync
    /// </summary>
    /// <param name="domainEvent"></param>
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// RemoveDomainEvent
    /// </summary>
    /// <param name="domainEvent"></param>
    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// ClearDomainEvents
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
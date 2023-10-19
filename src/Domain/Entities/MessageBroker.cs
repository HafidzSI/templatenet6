using System;

namespace NetCa.Domain.Entities;

/// <summary>
/// MessageBroker
/// </summary>
public record MessageBroker
{
    /// <summary>
    /// Gets or sets Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets Topic
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets Message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets StoredDate
    /// </summary>
    public DateTime? StoredDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether IsSend
    /// </summary>
    public bool IsSend { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Acknowledge
    /// </summary>
    public bool Acknowledged { get; set; }
}
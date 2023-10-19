using System;

namespace NetCa.Domain.Entities;

/// <summary>
/// ReceivedMessageBroker
/// </summary>
public record ReceivedMessageBroker
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
    /// Gets or sets Error
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets TimeIn
    /// </summary>
    public DateTime TimeIn { get; set; }

    /// <summary>
    /// Gets or sets Offset
    /// </summary>
    public long Offset { get; set; }

    /// <summary>
    /// Gets or sets Partition
    /// </summary>
    public int Partition { get; set; }

    /// <summary>
    /// Gets or sets Status
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Gets or sets InnerMessage
    /// </summary>
    public string InnerMessage { get; set; }

    /// <summary>
    /// Gets or sets StackTrace
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Gets or sets TimeProcess
    /// </summary>
    public DateTime? TimeProcess { get; set; }

    /// <summary>
    /// Gets or sets TimeFinish
    /// </summary>
    public DateTime? TimeFinish { get; set; }
}
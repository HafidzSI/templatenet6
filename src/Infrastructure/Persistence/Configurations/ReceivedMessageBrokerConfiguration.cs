using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Domain.Entities;

namespace NetCa.Infrastructure.Persistence.Configurations;

/// <summary>
/// ReceivedMessageBrokerConfiguration
/// </summary>
public class ReceivedMessageBrokerConfiguration : IEntityTypeConfiguration<ReceivedMessageBroker>
{
    /// <summary>
    /// Configure ReceivedMessageBroker
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<ReceivedMessageBroker> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Topic)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(e => e.Message)
            .HasColumnType("text");

        builder.Property(e => e.TimeIn)
            .HasColumnType("timestamp");

        builder.Property(e => e.TimeProcess)
            .HasColumnType("timestamp");

        builder.Property(e => e.TimeFinish)
            .HasColumnType("timestamp");

        builder.Property(e => e.Error)
            .HasColumnType("text");

        builder.Property(e => e.InnerMessage)
            .HasColumnType("text");

        builder.Property(e => e.StackTrace)
            .HasColumnType("text");

        builder.Property(e => e.Offset)
            .HasColumnType("bigint");

        builder.Property(e => e.Partition)
            .HasColumnType("int");

        builder.Property(e => e.Status)
            .HasColumnType("int");
    }
}
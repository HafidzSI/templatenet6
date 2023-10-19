using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Domain.Entities;

namespace NetCa.Infrastructure.Persistence.Configurations;

/// <summary>
/// MessageBrokerConfiguration
/// </summary>
public class MessageBrokerConfiguration : IEntityTypeConfiguration<MessageBroker>
{
    /// <summary>
    /// Configure KafkaMessage
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<MessageBroker> builder)
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

        builder.Property(e => e.StoredDate)
            .HasColumnType("Timestamp");

        builder.Property(e => e.IsSend)
            .HasColumnType("bool");

        builder.Property(e => e.Acknowledged)
            .HasColumnType("bool");
    }
}
// -----------------------------------------------------------------------------------
// AuditTableConfiguration.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Persistence.Configurations;

/// <summary>
/// AuditTableConfiguration
/// </summary>
/// <typeparam name="TBase"></typeparam>
public abstract class AuditTableConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : BaseAuditableEntity
{
    /// <summary>
    /// Configure for all entities
    /// </summary>
    /// <param name="builder"></param>
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasQueryFilter(e => e.IsActive.Value);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.IsActive)
            .HasColumnType("bool")
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedBy)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(e => e.CreatedDate)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("now()");

        builder.Property(e => e.UpdatedBy)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(e => e.UpdatedDate)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("now()");

        builder.HasIndex(b => b.IsActive);
    }
}

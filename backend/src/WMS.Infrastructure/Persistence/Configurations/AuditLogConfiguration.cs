using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "audit");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Username)
            .HasMaxLength(100);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .HasMaxLength(50);

        builder.Property(a => a.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => new { a.EntityName, a.EntityId });
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Domain)
            .HasMaxLength(100);

        builder.Property(t => t.ContactEmail)
            .HasMaxLength(100);

        builder.Property(t => t.ContactPhone)
            .HasMaxLength(20);

        builder.Property(t => t.Address)
            .HasMaxLength(500);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.HasIndex(t => t.Domain)
            .IsUnique()
            .HasFilter("[Domain] IS NOT NULL");
    }
}

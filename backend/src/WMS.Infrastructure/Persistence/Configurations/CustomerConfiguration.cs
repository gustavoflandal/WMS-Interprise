using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        // Informações Básicas
        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Tipo)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("PJ");

        builder.Property(c => c.NumeroDocumento)
            .HasMaxLength(18);

        // Contato
        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        // Status
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(CustomerStatus.Ativo);

        // Auditoria
        builder.Property(c => c.CriadoPor)
            .HasColumnType("uuid");

        builder.Property(c => c.AtualizadoPor)
            .HasColumnType("uuid");

        // Indexes
        builder.HasIndex(c => new { c.TenantId, c.NumeroDocumento })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false AND \"NumeroDocumento\" IS NOT NULL");

        builder.HasIndex(c => c.TenantId);

        builder.HasIndex(c => c.IsDeleted);

        builder.HasIndex(c => c.Status);

        // Relacionamento com Tenant
        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

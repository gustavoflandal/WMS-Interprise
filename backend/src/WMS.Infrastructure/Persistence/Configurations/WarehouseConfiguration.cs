using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(w => w.Id);

        // Informações Básicas
        builder.Property(w => w.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(w => w.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Descricao)
            .HasColumnType("text");

        // Localização
        builder.Property(w => w.Endereco)
            .HasMaxLength(500);

        builder.Property(w => w.Cidade)
            .HasMaxLength(100);

        builder.Property(w => w.Estado)
            .HasMaxLength(2);

        builder.Property(w => w.CEP)
            .HasMaxLength(10);

        builder.Property(w => w.Pais)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("BRA");

        builder.Property(w => w.Latitude)
            .HasColumnType("decimal(10,8)");

        builder.Property(w => w.Longitude)
            .HasColumnType("decimal(11,8)");

        // Capacidade
        builder.Property(w => w.TotalPosicoes)
            .HasColumnType("int");

        builder.Property(w => w.CapacidadePesoTotal)
            .HasColumnType("decimal(15,2)");

        // Operação
        builder.Property(w => w.HorarioAbertura)
            .HasColumnType("time");

        builder.Property(w => w.HorarioFechamento)
            .HasColumnType("time");

        builder.Property(w => w.MaxTrabalhadores)
            .HasColumnType("int");

        // Status
        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(WarehouseStatus.Ativo);

        // Auditoria
        builder.Property(w => w.CriadoPor)
            .HasColumnType("uuid");

        builder.Property(w => w.AtualizadoPor)
            .HasColumnType("uuid");

        // Indexes
        builder.HasIndex(w => new { w.TenantId, w.Codigo })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(w => w.TenantId);

        builder.HasIndex(w => w.IsDeleted);

        builder.HasIndex(w => w.Status);

        // Relacionamento com Tenant
        builder.HasOne(w => w.Tenant)
            .WithMany()
            .HasForeignKey(w => w.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        // Dados Principais
        builder.Property(c => c.RazaoSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.NomeFantasia)
            .HasMaxLength(200);

        builder.Property(c => c.CNPJ)
            .IsRequired()
            .HasMaxLength(18); // 00.000.000/0000-00

        builder.Property(c => c.InscricaoEstadual)
            .HasMaxLength(20);

        builder.Property(c => c.InscricaoMunicipal)
            .HasMaxLength(20);

        // Informações de Contato
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.Celular)
            .HasMaxLength(20);

        builder.Property(c => c.Website)
            .HasMaxLength(200);

        // Endereço
        builder.Property(c => c.CEP)
            .IsRequired()
            .HasMaxLength(10); // 00000-000

        builder.Property(c => c.Logradouro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Complemento)
            .HasMaxLength(100);

        builder.Property(c => c.Bairro)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Cidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(2); // UF

        // Informações Adicionais
        builder.Property(c => c.DataAbertura)
            .HasColumnType("date");

        builder.Property(c => c.CapitalSocial)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.AtividadePrincipal)
            .HasMaxLength(500);

        builder.Property(c => c.RegimeTributario)
            .HasMaxLength(50);

        // Responsável Legal
        builder.Property(c => c.NomeResponsavel)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CPFResponsavel)
            .IsRequired()
            .HasMaxLength(14); // 000.000.000-00

        builder.Property(c => c.CargoResponsavel)
            .HasMaxLength(100);

        builder.Property(c => c.EmailResponsavel)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.TelefoneResponsavel)
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(c => c.CNPJ)
            .IsUnique();

        builder.HasIndex(c => c.TenantId);

        builder.HasIndex(c => c.IsDeleted);

        // Relacionamento com Tenant
        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

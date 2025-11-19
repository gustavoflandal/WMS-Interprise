using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WMS.API.Temp;

public partial class WmsDevContext : DbContext
{
    public WmsDevContext(DbContextOptions<WmsDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Companies", "wms");

            entity.HasIndex(e => e.Cnpj, "IX_Companies_CNPJ").IsUnique();

            entity.HasIndex(e => e.IsDeleted, "IX_Companies_IsDeleted");

            entity.HasIndex(e => e.TenantId, "IX_Companies_TenantId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AtividadePrincipal).HasMaxLength(500);
            entity.Property(e => e.Bairro).HasMaxLength(100);
            entity.Property(e => e.CapitalSocial).HasPrecision(18, 2);
            entity.Property(e => e.CargoResponsavel).HasMaxLength(100);
            entity.Property(e => e.Celular).HasMaxLength(20);
            entity.Property(e => e.Cep)
                .HasMaxLength(10)
                .HasColumnName("CEP");
            entity.Property(e => e.Cidade).HasMaxLength(100);
            entity.Property(e => e.Cnpj)
                .HasMaxLength(18)
                .HasColumnName("CNPJ");
            entity.Property(e => e.Complemento).HasMaxLength(100);
            entity.Property(e => e.Cpfresponsavel)
                .HasMaxLength(14)
                .HasColumnName("CPFResponsavel");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmailResponsavel).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.InscricaoEstadual).HasMaxLength(20);
            entity.Property(e => e.InscricaoMunicipal).HasMaxLength(20);
            entity.Property(e => e.Logradouro).HasMaxLength(200);
            entity.Property(e => e.NomeFantasia).HasMaxLength(200);
            entity.Property(e => e.NomeResponsavel).HasMaxLength(200);
            entity.Property(e => e.Numero).HasMaxLength(20);
            entity.Property(e => e.RazaoSocial).HasMaxLength(200);
            entity.Property(e => e.RegimeTributario).HasMaxLength(50);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.TelefoneResponsavel).HasMaxLength(20);
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

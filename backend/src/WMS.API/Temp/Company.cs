using System;
using System.Collections.Generic;

namespace WMS.API.Temp;

public partial class Company
{
    public Guid Id { get; set; }

    public string RazaoSocial { get; set; } = null!;

    public string? NomeFantasia { get; set; }

    public string Cnpj { get; set; } = null!;

    public string? InscricaoEstadual { get; set; }

    public string? InscricaoMunicipal { get; set; }

    public string Email { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Celular { get; set; }

    public string? Website { get; set; }

    public string Cep { get; set; } = null!;

    public string Logradouro { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateOnly? DataAbertura { get; set; }

    public decimal? CapitalSocial { get; set; }

    public string? AtividadePrincipal { get; set; }

    public string? RegimeTributario { get; set; }

    public string NomeResponsavel { get; set; } = null!;

    public string Cpfresponsavel { get; set; } = null!;

    public string? CargoResponsavel { get; set; }

    public string EmailResponsavel { get; set; } = null!;

    public string? TelefoneResponsavel { get; set; }

    public Guid TenantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }
}

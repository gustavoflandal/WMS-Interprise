namespace WMS.Domain.Entities;

/// <summary>
/// Representa um cliente no sistema WMS.
/// Cada tenant pode ter múltiplos clientes com características específicas.
/// </summary>
public class Customer : BaseEntity
{
    // Informações Básicas
    public string Nome { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "PJ"; // "PJ" ou "PF"
    public string? NumeroDocumento { get; private set; }

    // Contato
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }

    // Status
    public CustomerStatus Status { get; private set; }

    // Auditoria
    public Guid? CriadoPor { get; private set; }
    public Guid? AtualizadoPor { get; private set; }

    // Relacionamento com Tenant
    public Guid TenantId { get; private set; }
    public virtual Tenant Tenant { get; private set; } = null!;

    private Customer() { } // Para o EF Core

    public Customer(
        Guid tenantId,
        string nome,
        string tipo = "PJ",
        string? numeroDocumento = null,
        string? email = null,
        string? telefone = null,
        Guid? criadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório", nameof(nome));

        if (tipo != "PJ" && tipo != "PF")
            throw new ArgumentException("Tipo deve ser 'PJ' ou 'PF'", nameof(tipo));

        TenantId = tenantId;
        Nome = nome;
        Tipo = tipo;
        NumeroDocumento = numeroDocumento;
        Email = email;
        Telefone = telefone;
        Status = CustomerStatus.Ativo;
        CriadoPor = criadoPor;
    }

    public void UpdateInfo(
        string nome,
        string tipo,
        string? numeroDocumento,
        string? email,
        string? telefone,
        Guid? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório", nameof(nome));

        if (tipo != "PJ" && tipo != "PF")
            throw new ArgumentException("Tipo deve ser 'PJ' ou 'PF'", nameof(tipo));

        Nome = nome;
        Tipo = tipo;
        NumeroDocumento = numeroDocumento;
        Email = email;
        Telefone = telefone;
        AtualizadoPor = atualizadoPor;
    }

    public void UpdateStatus(CustomerStatus novoStatus, Guid? atualizadoPor = null)
    {
        Status = novoStatus;
        AtualizadoPor = atualizadoPor;
    }

    public void Activate(Guid? atualizadoPor = null)
    {
        UpdateStatus(CustomerStatus.Ativo, atualizadoPor);
    }

    public void Deactivate(Guid? atualizadoPor = null)
    {
        UpdateStatus(CustomerStatus.Inativo, atualizadoPor);
    }

    public void Block(Guid? atualizadoPor = null)
    {
        UpdateStatus(CustomerStatus.Bloqueado, atualizadoPor);
    }

    public bool IsActive() => Status == CustomerStatus.Ativo;
}

/// <summary>
/// Status possíveis para um cliente
/// </summary>
public enum CustomerStatus
{
    Ativo = 1,
    Inativo = 2,
    Bloqueado = 3
}

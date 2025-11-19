namespace WMS.Domain.Entities;

/// <summary>
/// Representa um armazém no sistema WMS.
/// Cada tenant pode ter múltiplos armazéns com características específicas.
/// </summary>
public class Warehouse : BaseEntity
{
    // Informações Básicas
    public string Nome { get; private set; } = string.Empty;
    public string Codigo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    // Localização
    public string? Endereco { get; private set; }
    public string? Cidade { get; private set; }
    public string? Estado { get; private set; }
    public string? CEP { get; private set; }
    public string Pais { get; private set; } = "BRA";
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }

    // Capacidade
    public int? TotalPosicoes { get; private set; }
    public decimal? CapacidadePesoTotal { get; private set; } // em kg

    // Operação
    public TimeSpan? HorarioAbertura { get; private set; }
    public TimeSpan? HorarioFechamento { get; private set; }
    public int? MaxTrabalhadores { get; private set; }

    // Status
    public WarehouseStatus Status { get; private set; }

    // Auditoria
    public Guid? CriadoPor { get; private set; }
    public Guid? AtualizadoPor { get; private set; }

    // Relacionamento com Tenant
    public Guid TenantId { get; private set; }
    public virtual Tenant Tenant { get; private set; } = null!;

    private Warehouse() { } // Para o EF Core

    public Warehouse(
        Guid tenantId,
        string nome,
        string codigo,
        string? descricao = null,
        Guid? criadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do armazém é obrigatório", nameof(nome));
        
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código do armazém é obrigatório", nameof(codigo));
        
        TenantId = tenantId;
        Nome = nome;
        Codigo = codigo;
        Descricao = descricao;
        Status = WarehouseStatus.Ativo;
        CriadoPor = criadoPor;
        Pais = "BRA";
    }

    public void UpdateInfo(
        string nome,
        string? descricao,
        string? endereco,
        string? cidade,
        string? estado,
        string? cep,
        string pais,
        decimal? latitude,
        decimal? longitude,
        int? totalPosicoes,
        decimal? capacidadePesoTotal,
        TimeSpan? horarioAbertura,
        TimeSpan? horarioFechamento,
        int? maxTrabalhadores,
        Guid? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do armazém é obrigatório", nameof(nome));

        Nome = nome;
        Descricao = descricao;
        Endereco = endereco;
        Cidade = cidade;
        Estado = estado;
        CEP = cep;
        Pais = pais ?? "BRA";
        Latitude = latitude;
        Longitude = longitude;
        TotalPosicoes = totalPosicoes;
        CapacidadePesoTotal = capacidadePesoTotal;
        HorarioAbertura = horarioAbertura;
        HorarioFechamento = horarioFechamento;
        MaxTrabalhadores = maxTrabalhadores;
        AtualizadoPor = atualizadoPor;
    }

    public void UpdateStatus(WarehouseStatus novoStatus, Guid? atualizadoPor = null)
    {
        Status = novoStatus;
        AtualizadoPor = atualizadoPor;
    }

    public void Activate(Guid? atualizadoPor = null)
    {
        UpdateStatus(WarehouseStatus.Ativo, atualizadoPor);
    }

    public void Deactivate(Guid? atualizadoPor = null)
    {
        UpdateStatus(WarehouseStatus.Inativo, atualizadoPor);
    }

    public void SetMaintenance(Guid? atualizadoPor = null)
    {
        UpdateStatus(WarehouseStatus.EmManutencao, atualizadoPor);
    }

    public bool IsOperational() => Status == WarehouseStatus.Ativo;

    public bool CanReceiveGoods() => Status == WarehouseStatus.Ativo || Status == WarehouseStatus.EmManutencao;
}

/// <summary>
/// Status possíveis para um armazém
/// </summary>
public enum WarehouseStatus
{
    Ativo = 1,
    Inativo = 2,
    EmManutencao = 3
}

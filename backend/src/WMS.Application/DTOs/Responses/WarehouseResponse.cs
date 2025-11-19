namespace WMS.Application.DTOs.Responses;

public record WarehouseResponse(
    Guid Id,
    
    // Informações Básicas
    string Nome,
    string Codigo,
    string? Descricao,

    // Localização
    string? Endereco,
    string? Cidade,
    string? Estado,
    string? CEP,
    string Pais,
    decimal? Latitude,
    decimal? Longitude,

    // Capacidade
    int? TotalPosicoes,
    decimal? CapacidadePesoTotal,

    // Operação
    TimeSpan? HorarioAbertura,
    TimeSpan? HorarioFechamento,
    int? MaxTrabalhadores,

    // Status
    int Status, // 1=Ativo, 2=Inativo, 3=EmManutencao
    string StatusDescricao,

    // Auditoria
    Guid? CriadoPor,
    Guid? AtualizadoPor,

    // Metadata
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

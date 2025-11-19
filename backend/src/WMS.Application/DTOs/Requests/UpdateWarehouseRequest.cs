namespace WMS.Application.DTOs.Requests;

public record UpdateWarehouseRequest(
    // Informações Básicas
    string Nome,
    string? Descricao = null,

    // Localização
    string? Endereco = null,
    string? Cidade = null,
    string? Estado = null,
    string? CEP = null,
    string? Pais = "BRA",
    decimal? Latitude = null,
    decimal? Longitude = null,

    // Capacidade
    int? TotalPosicoes = null,
    decimal? CapacidadePesoTotal = null,

    // Operação
    TimeSpan? HorarioAbertura = null,
    TimeSpan? HorarioFechamento = null,
    int? MaxTrabalhadores = null,
    
    // Status
    int Status = 1 // 1=Ativo, 2=Inativo, 3=EmManutencao
);

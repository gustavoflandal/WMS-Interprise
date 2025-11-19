namespace WMS.Application.DTOs.Requests;

public record CreateWarehouseRequest(
    // Informações Básicas
    string Nome,
    string Codigo,
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
    int? MaxTrabalhadores = null
);

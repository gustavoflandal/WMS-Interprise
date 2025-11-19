using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record CreateWarehouseRequest(
    // Informações Básicas
    [Required(ErrorMessage = "O nome do armazém é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    string Nome,

    [Required(ErrorMessage = "O código do armazém é obrigatório")]
    [StringLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres")]
    string Codigo,

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    string? Descricao = null,

    // Localização
    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
    string? Endereco = null,

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    string? Cidade = null,

    [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres")]
    string? Estado = null,

    [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter 8 caracteres")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter apenas números")]
    string? CEP = null,

    [StringLength(3, MinimumLength = 3, ErrorMessage = "O código do país deve ter 3 caracteres")]
    string? Pais = "BRA",

    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90")]
    decimal? Latitude = null,

    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180")]
    decimal? Longitude = null,

    // Capacidade
    [Range(1, int.MaxValue, ErrorMessage = "O total de posições deve ser maior que zero")]
    int? TotalPosicoes = null,

    [Range(0, double.MaxValue, ErrorMessage = "A capacidade de peso deve ser maior ou igual a zero")]
    decimal? CapacidadePesoTotal = null,

    // Operação
    TimeSpan? HorarioAbertura = null,
    TimeSpan? HorarioFechamento = null,

    [Range(1, int.MaxValue, ErrorMessage = "O número máximo de trabalhadores deve ser maior que zero")]
    int? MaxTrabalhadores = null
);

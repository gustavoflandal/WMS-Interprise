using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

/// <summary>
/// DTO para criar uma nova Localização de Armazenamento
/// </summary>
public class CreateStorageLocationRequest
{
    /// <summary>
    /// ID do armazém
    /// </summary>
    [Required(ErrorMessage = "ID do armazém é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do armazém inválido")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// Código da localização (ex: A-001-01-01)
    /// </summary>
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Código deve ter entre 5 e 50 caracteres")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Zona de armazenamento (0-9)
    /// </summary>
    [Required(ErrorMessage = "Zona é obrigatória")]
    [Range(0, 9, ErrorMessage = "Zona inválida")]
    public int Zone { get; set; }

    /// <summary>
    /// Capacidade máxima em kg
    /// </summary>
    [Required(ErrorMessage = "Capacidade em kg é obrigatória")]
    [Range(0.1, 100000, ErrorMessage = "Capacidade deve estar entre 0.1 e 100000 kg")]
    public decimal MaxCapacityKg { get; set; }

    /// <summary>
    /// Capacidade máxima em unidades
    /// </summary>
    [Required(ErrorMessage = "Capacidade em unidades é obrigatória")]
    [Range(1, 1000000, ErrorMessage = "Capacidade deve estar entre 1 e 1000000 unidades")]
    public int MaxCapacityUnits { get; set; }

    /// <summary>
    /// Posição na linha
    /// </summary>
    [Required(ErrorMessage = "Posição de linha é obrigatória")]
    [Range(1, 1000, ErrorMessage = "Posição deve estar entre 1 e 1000")]
    public int RowPosition { get; set; }

    /// <summary>
    /// Posição na coluna
    /// </summary>
    [Required(ErrorMessage = "Posição de coluna é obrigatória")]
    [Range(1, 1000, ErrorMessage = "Posição deve estar entre 1 e 1000")]
    public int ColumnPosition { get; set; }

    /// <summary>
    /// Posição do nível
    /// </summary>
    [Required(ErrorMessage = "Posição de nível é obrigatória")]
    [Range(1, 100, ErrorMessage = "Posição deve estar entre 1 e 100")]
    public int LevelPosition { get; set; }

    /// <summary>
    /// Número do corredor
    /// </summary>
    [Required(ErrorMessage = "Número do corredor é obrigatório")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Número deve ter entre 1 e 50 caracteres")]
    public string CorridorNumber { get; set; } = string.Empty;

    /// <summary>
    /// Distância do ponto de consolidação
    /// </summary>
    [Required(ErrorMessage = "Distância é obrigatória")]
    [Range(0, 10000, ErrorMessage = "Distância deve estar entre 0 e 10000 metros")]
    public decimal DistanceFromConsolidationPoint { get; set; }

    /// <summary>
    /// Se a localização requer controle de temperatura
    /// </summary>
    public bool RequiresTemperatureControl { get; set; } = false;

    /// <summary>
    /// Temperatura mínima (se requer controle)
    /// </summary>
    [Range(-50, 50, ErrorMessage = "Temperatura deve estar entre -50°C e 50°C")]
    public decimal? MinTemperature { get; set; }

    /// <summary>
    /// Temperatura máxima (se requer controle)
    /// </summary>
    [Range(-50, 50, ErrorMessage = "Temperatura deve estar entre -50°C e 50°C")]
    public decimal? MaxTemperature { get; set; }

    /// <summary>
    /// Umidade mínima (se requer controle)
    /// </summary>
    [Range(0, 100, ErrorMessage = "Umidade deve estar entre 0% e 100%")]
    public decimal? MinHumidity { get; set; }

    /// <summary>
    /// Umidade máxima (se requer controle)
    /// </summary>
    [Range(0, 100, ErrorMessage = "Umidade deve estar entre 0% e 100%")]
    public decimal? MaxHumidity { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

/// <summary>
/// DTO para atualizar um Produto existente
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// Nome do produto
    /// </summary>
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 255 caracteres")]
    public string? Name { get; set; }

    /// <summary>
    /// Categoria do produto (0-7)
    /// </summary>
    [Range(0, 7, ErrorMessage = "Categoria inválida")]
    public int? Category { get; set; }

    /// <summary>
    /// Tipo do produto (0-7)
    /// </summary>
    [Range(0, 7, ErrorMessage = "Tipo inválido")]
    public int? Type { get; set; }

    /// <summary>
    /// Peso unitário em kg
    /// </summary>
    [Range(0.01, 10000, ErrorMessage = "Peso deve estar entre 0.01 e 10000 kg")]
    public decimal? UnitWeight { get; set; }

    /// <summary>
    /// Volume unitário em m³
    /// </summary>
    [Range(0.0001, 1000, ErrorMessage = "Volume deve estar entre 0.0001 e 1000 m³")]
    public decimal? UnitVolume { get; set; }

    /// <summary>
    /// Zona de armazenamento padrão (0-9)
    /// </summary>
    [Range(0, 9, ErrorMessage = "Zona de armazenamento inválida")]
    public int? DefaultStorageZone { get; set; }

    /// <summary>
    /// Se o produto requer rastreamento de lote
    /// </summary>
    public bool? RequiresLotTracking { get; set; }

    /// <summary>
    /// Se o produto requer número de série
    /// </summary>
    public bool? RequiresSerialNumber { get; set; }

    /// <summary>
    /// Dias de validade (shelf life)
    /// </summary>
    [Range(1, 3650, ErrorMessage = "Validade deve estar entre 1 e 3650 dias")]
    public int? ShelfLifeDays { get; set; }

    /// <summary>
    /// Temperatura mínima de armazenamento
    /// </summary>
    [Range(-50, 50, ErrorMessage = "Temperatura deve estar entre -50°C e 50°C")]
    public decimal? MinStorageTemperature { get; set; }

    /// <summary>
    /// Temperatura máxima de armazenamento
    /// </summary>
    [Range(-50, 50, ErrorMessage = "Temperatura deve estar entre -50°C e 50°C")]
    public decimal? MaxStorageTemperature { get; set; }

    /// <summary>
    /// Umidade mínima de armazenamento (%)
    /// </summary>
    [Range(0, 100, ErrorMessage = "Umidade deve estar entre 0% e 100%")]
    public decimal? MinStorageHumidity { get; set; }

    /// <summary>
    /// Umidade máxima de armazenamento (%)
    /// </summary>
    [Range(0, 100, ErrorMessage = "Umidade deve estar entre 0% e 100%")]
    public decimal? MaxStorageHumidity { get; set; }

    /// <summary>
    /// Se o produto é inflamável
    /// </summary>
    public bool? IsFlammable { get; set; }

    /// <summary>
    /// Se o produto é perigoso
    /// </summary>
    public bool? IsDangerous { get; set; }

    /// <summary>
    /// Se o produto é farmacêutico
    /// </summary>
    public bool? IsPharmaceutical { get; set; }

    /// <summary>
    /// Classificação ABC do produto
    /// </summary>
    [Range(0, 2, ErrorMessage = "Classificação ABC inválida")]
    public int? ABCClassification { get; set; }

    /// <summary>
    /// Custo unitário
    /// </summary>
    [Range(0.01, 1000000, ErrorMessage = "Custo deve estar entre 0.01 e 1000000")]
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// Preço unitário de venda
    /// </summary>
    [Range(0.01, 1000000, ErrorMessage = "Preço deve estar entre 0.01 e 1000000")]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Se o produto está ativo
    /// </summary>
    public bool? IsActive { get; set; }
}

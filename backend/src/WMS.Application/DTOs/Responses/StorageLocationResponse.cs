namespace WMS.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para Localização de Armazenamento
/// </summary>
public class StorageLocationResponse
{
    public Guid Id { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Zone { get; set; }
    public int Status { get; set; }
    public int? CurrentProductId { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal MaxCapacityKg { get; set; }
    public int MaxCapacityUnits { get; set; }
    public int RowPosition { get; set; }
    public int ColumnPosition { get; set; }
    public int LevelPosition { get; set; }
    public string CorridorNumber { get; set; } = string.Empty;
    public decimal DistanceFromConsolidationPoint { get; set; }
    public bool RequiresTemperatureControl { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public decimal? MinHumidity { get; set; }
    public decimal? MaxHumidity { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public int AccessCount { get; set; }
    public DateTime? LastRemovalDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

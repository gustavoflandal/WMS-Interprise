namespace WMS.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para Produto
/// </summary>
public class ProductResponse
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Category { get; set; }
    public int Type { get; set; }
    public decimal UnitWeight { get; set; }
    public decimal UnitVolume { get; set; }
    public int DefaultStorageZone { get; set; }
    public bool RequiresLotTracking { get; set; }
    public bool RequiresSerialNumber { get; set; }
    public int? ShelfLifeDays { get; set; }
    public decimal? MinStorageTemperature { get; set; }
    public decimal? MaxStorageTemperature { get; set; }
    public decimal? MinStorageHumidity { get; set; }
    public decimal? MaxStorageHumidity { get; set; }
    public bool IsFlammable { get; set; }
    public bool IsDangerous { get; set; }
    public bool IsPharmaceutical { get; set; }
    public int? ABCClassification { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

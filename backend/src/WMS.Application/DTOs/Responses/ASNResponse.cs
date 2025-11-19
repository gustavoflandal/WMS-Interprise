namespace WMS.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para ASN (Advance Shipping Notice)
/// </summary>
public class ASNResponse
{
    public Guid Id { get; set; }
    public string AsnNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public int? SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime ScheduledArrivalDate { get; set; }
    public DateTime? ActualArrivalDate { get; set; }
    public int Status { get; set; }
    public int ExpectedItemCount { get; set; }
    public int ReceivedItemCount { get; set; }
    public decimal ExpectedTotalWeight { get; set; }
    public decimal? ExpectedTotalVolume { get; set; }
    public int Priority { get; set; }
    public bool IsInspected { get; set; }
    public int? InspectionResult { get; set; }
    public string? ExternalReference { get; set; }
    public int? OriginWarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

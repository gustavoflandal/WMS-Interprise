namespace WMS.Application.DTOs.Responses;

/// <summary>
/// DTO de resposta para Recebimento
/// </summary>
public class ReceiptResponse
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public int? AsnId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public int Status { get; set; }
    public int ReceiptType { get; set; }
    public int TotalItemsReceived { get; set; }
    public decimal TotalWeight { get; set; }
    public int OperatorId { get; set; }
    public int? SupervisorId { get; set; }
    public string? InvoiceNumber { get; set; }
    public bool HasDiscrepancies { get; set; }
    public int? TotalReceiptTimeMinutes { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? ExternalReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

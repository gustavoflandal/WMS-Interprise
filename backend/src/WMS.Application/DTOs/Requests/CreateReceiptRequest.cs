using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

/// <summary>
/// DTO para criar um novo Recebimento
/// </summary>
public class CreateReceiptRequest
{
    /// <summary>
    /// ID do armazém
    /// </summary>
    [Required(ErrorMessage = "ID do armazém é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do armazém inválido")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// ID da ASN associada (opcional)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "ID da ASN inválido")]
    public int? AsnId { get; set; }

    /// <summary>
    /// Tipo de recebimento (0-5)
    /// </summary>
    [Required(ErrorMessage = "Tipo de recebimento é obrigatório")]
    [Range(0, 5, ErrorMessage = "Tipo de recebimento inválido")]
    public int ReceiptType { get; set; }

    /// <summary>
    /// Número da nota fiscal
    /// </summary>
    [StringLength(100, ErrorMessage = "Número deve ter no máximo 100 caracteres")]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// ID do operador
    /// </summary>
    [Required(ErrorMessage = "ID do operador é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do operador inválido")]
    public int OperatorId { get; set; }

    /// <summary>
    /// ID do supervisor (opcional)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "ID do supervisor inválido")]
    public int? SupervisorId { get; set; }

    /// <summary>
    /// Referência externa
    /// </summary>
    [StringLength(255, ErrorMessage = "Referência deve ter no máximo 255 caracteres")]
    public string? ExternalReference { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

/// <summary>
/// DTO para criar um novo ASN (Advance Shipping Notice)
/// </summary>
public class CreateASNRequest
{
    /// <summary>
    /// Número da ASN
    /// </summary>
    [Required(ErrorMessage = "Número da ASN é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Número deve ter entre 3 e 100 caracteres")]
    public string AsnNumber { get; set; } = string.Empty;

    /// <summary>
    /// ID do armazém
    /// </summary>
    [Required(ErrorMessage = "ID do armazém é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do armazém inválido")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// ID do fornecedor
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "ID do fornecedor inválido")]
    public int? SupplierId { get; set; }

    /// <summary>
    /// Número da nota fiscal
    /// </summary>
    [StringLength(100, ErrorMessage = "Número deve ter no máximo 100 caracteres")]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Data de chegada agendada
    /// </summary>
    [Required(ErrorMessage = "Data de chegada é obrigatória")]
    public DateTime ScheduledArrivalDate { get; set; }

    /// <summary>
    /// Quantidade esperada de itens
    /// </summary>
    [Required(ErrorMessage = "Quantidade de itens é obrigatória")]
    [Range(1, 1000000, ErrorMessage = "Quantidade deve ser maior que 0")]
    public int ExpectedItemCount { get; set; }

    /// <summary>
    /// Peso total esperado em kg
    /// </summary>
    [Required(ErrorMessage = "Peso total é obrigatório")]
    [Range(0.01, 1000000, ErrorMessage = "Peso deve estar entre 0.01 e 1000000 kg")]
    public decimal ExpectedTotalWeight { get; set; }

    /// <summary>
    /// Volume total esperado em m³
    /// </summary>
    [Range(0.0001, 100000, ErrorMessage = "Volume deve estar entre 0.0001 e 100000 m³")]
    public decimal? ExpectedTotalVolume { get; set; }

    /// <summary>
    /// Prioridade da ASN (0-3)
    /// </summary>
    [Required(ErrorMessage = "Prioridade é obrigatória")]
    [Range(0, 3, ErrorMessage = "Prioridade inválida")]
    public int Priority { get; set; }

    /// <summary>
    /// Referência externa (ID do fornecedor, PO, etc)
    /// </summary>
    [StringLength(255, ErrorMessage = "Referência deve ter no máximo 255 caracteres")]
    public string? ExternalReference { get; set; }

    /// <summary>
    /// ID do armazém de origem (para transferências internas)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "ID do armazém inválido")]
    public int? OriginWarehouseId { get; set; }
}

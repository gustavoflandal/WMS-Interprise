using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record UpdateCustomerRequest(
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    string Nome,

    [Required(ErrorMessage = "O tipo é obrigatório")]
    [RegularExpression("^(PF|PJ)$", ErrorMessage = "O tipo deve ser 'PF' (Pessoa Física) ou 'PJ' (Pessoa Jurídica)")]
    string Tipo,

    [StringLength(14, ErrorMessage = "O número do documento deve ter no máximo 14 caracteres (11 para CPF, 14 para CNPJ)")]
    string? NumeroDocumento = null,

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    string? Email = null,

    [Phone(ErrorMessage = "Número de telefone inválido")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    string? Telefone = null,

    [Range(1, 2, ErrorMessage = "O status deve ser 1 (Ativo) ou 2 (Inativo)")]
    int Status = 1
);

using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record CreateCompanyRequest(
    // Dados Principais
    [Required(ErrorMessage = "A razão social é obrigatória")]
    [StringLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
    string RazaoSocial,

    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 caracteres")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter apenas números")]
    string CNPJ,

    [StringLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
    string? NomeFantasia = null,

    [StringLength(20, ErrorMessage = "A inscrição estadual deve ter no máximo 20 caracteres")]
    string? InscricaoEstadual = null,

    [StringLength(20, ErrorMessage = "A inscrição municipal deve ter no máximo 20 caracteres")]
    string? InscricaoMunicipal = null,

    // Informações de Contato
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    string Email = "",

    [Phone(ErrorMessage = "Número de telefone inválido")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    string? Telefone = null,

    [Phone(ErrorMessage = "Número de celular inválido")]
    [StringLength(20, ErrorMessage = "O celular deve ter no máximo 20 caracteres")]
    string? Celular = null,

    [Url(ErrorMessage = "URL do website inválida")]
    [StringLength(200, ErrorMessage = "O website deve ter no máximo 200 caracteres")]
    string? Website = null,

    // Endereço
    [Required(ErrorMessage = "O CEP é obrigatório")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter 8 caracteres")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter apenas números")]
    string CEP = "",

    [Required(ErrorMessage = "O logradouro é obrigatório")]
    [StringLength(200, ErrorMessage = "O logradouro deve ter no máximo 200 caracteres")]
    string Logradouro = "",

    [Required(ErrorMessage = "O número é obrigatório")]
    [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
    string Numero = "",

    [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
    string? Complemento = null,

    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres")]
    string Bairro = "",

    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    string Cidade = "",

    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres")]
    string Estado = "",

    // Informações Adicionais
    DateTime? DataAbertura = null,

    [Range(0, double.MaxValue, ErrorMessage = "O capital social deve ser maior ou igual a zero")]
    decimal? CapitalSocial = null,

    [StringLength(500, ErrorMessage = "A atividade principal deve ter no máximo 500 caracteres")]
    string? AtividadePrincipal = null,

    [StringLength(50, ErrorMessage = "O regime tributário deve ter no máximo 50 caracteres")]
    string? RegimeTributario = null,

    // Responsável Legal
    [Required(ErrorMessage = "O nome do responsável é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do responsável deve ter no máximo 100 caracteres")]
    string NomeResponsavel = "",

    [Required(ErrorMessage = "O CPF do responsável é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
    string CPFResponsavel = "",

    [StringLength(50, ErrorMessage = "O cargo do responsável deve ter no máximo 50 caracteres")]
    string? CargoResponsavel = null,

    [Required(ErrorMessage = "O email do responsável é obrigatório")]
    [EmailAddress(ErrorMessage = "Email do responsável inválido")]
    [StringLength(100, ErrorMessage = "O email do responsável deve ter no máximo 100 caracteres")]
    string EmailResponsavel = "",

    [Phone(ErrorMessage = "Número de telefone do responsável inválido")]
    [StringLength(20, ErrorMessage = "O telefone do responsável deve ter no máximo 20 caracteres")]
    string? TelefoneResponsavel = null
);

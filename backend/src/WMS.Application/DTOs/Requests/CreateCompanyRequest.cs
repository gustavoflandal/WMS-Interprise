namespace WMS.Application.DTOs.Requests;

public record CreateCompanyRequest(
    // Dados Principais
    string RazaoSocial,
    string CNPJ,
    string? NomeFantasia = null,
    string? InscricaoEstadual = null,
    string? InscricaoMunicipal = null,

    // Informações de Contato
    string Email = "",
    string? Telefone = null,
    string? Celular = null,
    string? Website = null,

    // Endereço
    string CEP = "",
    string Logradouro = "",
    string Numero = "",
    string? Complemento = null,
    string Bairro = "",
    string Cidade = "",
    string Estado = "",

    // Informações Adicionais
    DateTime? DataAbertura = null,
    decimal? CapitalSocial = null,
    string? AtividadePrincipal = null,
    string? RegimeTributario = null,

    // Responsável Legal
    string NomeResponsavel = "",
    string CPFResponsavel = "",
    string? CargoResponsavel = null,
    string EmailResponsavel = "",
    string? TelefoneResponsavel = null
);

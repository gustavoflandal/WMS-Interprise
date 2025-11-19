namespace WMS.Application.DTOs.Requests;

public record UpdateCompanyRequest(
    // Dados Principais (CNPJ e RazaoSocial são imutáveis, não incluídos)
    string? NomeFantasia = null,
    string? InscricaoEstadual = null,
    string? InscricaoMunicipal = null,

    // Informações de Contato
    string? Email = null,
    string? Telefone = null,
    string? Celular = null,
    string? Website = null,

    // Endereço
    string? CEP = null,
    string? Logradouro = null,
    string? Numero = null,
    string? Complemento = null,
    string? Bairro = null,
    string? Cidade = null,
    string? Estado = null,

    // Informações Adicionais
    DateTime? DataAbertura = null,
    decimal? CapitalSocial = null,
    string? AtividadePrincipal = null,
    string? RegimeTributario = null,

    // Responsável Legal
    string? NomeResponsavel = null,
    string? CPFResponsavel = null,
    string? CargoResponsavel = null,
    string? EmailResponsavel = null,
    string? TelefoneResponsavel = null
);

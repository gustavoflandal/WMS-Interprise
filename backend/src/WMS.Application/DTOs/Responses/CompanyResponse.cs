namespace WMS.Application.DTOs.Responses;

public record CompanyResponse(
    Guid Id,
    
    // Dados Principais
    string RazaoSocial,
    string? NomeFantasia,
    string CNPJ,
    string? InscricaoEstadual,
    string? InscricaoMunicipal,

    // Informações de Contato
    string Email,
    string? Telefone,
    string? Celular,
    string? Website,

    // Endereço
    string CEP,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,

    // Informações Adicionais
    DateTime? DataAbertura,
    decimal? CapitalSocial,
    string? AtividadePrincipal,
    string? RegimeTributario,

    // Responsável Legal
    string NomeResponsavel,
    string CPFResponsavel,
    string? CargoResponsavel,
    string EmailResponsavel,
    string? TelefoneResponsavel,

    // Metadata
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

namespace WMS.Domain.Entities;

public class Company : BaseEntity
{
    // Dados Principais
    public string RazaoSocial { get; private set; } = string.Empty;
    public string? NomeFantasia { get; private set; }
    public string CNPJ { get; private set; } = string.Empty;
    public string? InscricaoEstadual { get; private set; }
    public string? InscricaoMunicipal { get; private set; }

    // Informações de Contato
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public string? Celular { get; private set; }
    public string? Website { get; private set; }

    // Endereço
    public string CEP { get; private set; } = string.Empty;
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;

    // Informações Adicionais
    public DateTime? DataAbertura { get; private set; }
    public decimal? CapitalSocial { get; private set; }
    public string? AtividadePrincipal { get; private set; }
    public string? RegimeTributario { get; private set; }

    // Responsável Legal
    public string NomeResponsavel { get; private set; } = string.Empty;
    public string CPFResponsavel { get; private set; } = string.Empty;
    public string? CargoResponsavel { get; private set; }
    public string EmailResponsavel { get; private set; } = string.Empty;
    public string? TelefoneResponsavel { get; private set; }

    // Relacionamento com Tenant
    public Guid TenantId { get; private set; }
    public virtual Tenant Tenant { get; private set; } = null!;

    private Company() { } // For EF Core

    public Company(
        Guid tenantId,
        string razaoSocial,
        string cnpj,
        string email,
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string nomeResponsavel,
        string cpfResponsavel,
        string emailResponsavel)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão social é obrigatória", nameof(razaoSocial));
        
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ é obrigatório", nameof(cnpj));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));
        
        if (string.IsNullOrWhiteSpace(nomeResponsavel))
            throw new ArgumentException("Nome do responsável é obrigatório", nameof(nomeResponsavel));
        
        if (string.IsNullOrWhiteSpace(cpfResponsavel))
            throw new ArgumentException("CPF do responsável é obrigatório", nameof(cpfResponsavel));
        
        if (string.IsNullOrWhiteSpace(emailResponsavel))
            throw new ArgumentException("Email do responsável é obrigatório", nameof(emailResponsavel));

        TenantId = tenantId;
        RazaoSocial = razaoSocial;
        CNPJ = cnpj;
        Email = email;
        CEP = cep;
        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        NomeResponsavel = nomeResponsavel;
        CPFResponsavel = cpfResponsavel;
        EmailResponsavel = emailResponsavel;
    }

    public void UpdateInfo(
        string? nomeFantasia,
        string? inscricaoEstadual,
        string? inscricaoMunicipal,
        string email,
        string? telefone,
        string? celular,
        string? website,
        string cep,
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        DateTime? dataAbertura,
        decimal? capitalSocial,
        string? atividadePrincipal,
        string? regimeTributario,
        string nomeResponsavel,
        string cpfResponsavel,
        string? cargoResponsavel,
        string emailResponsavel,
        string? telefoneResponsavel)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));
        
        if (string.IsNullOrWhiteSpace(nomeResponsavel))
            throw new ArgumentException("Nome do responsável é obrigatório", nameof(nomeResponsavel));
        
        if (string.IsNullOrWhiteSpace(cpfResponsavel))
            throw new ArgumentException("CPF do responsável é obrigatório", nameof(cpfResponsavel));
        
        if (string.IsNullOrWhiteSpace(emailResponsavel))
            throw new ArgumentException("Email do responsável é obrigatório", nameof(emailResponsavel));

        NomeFantasia = nomeFantasia;
        InscricaoEstadual = inscricaoEstadual;
        InscricaoMunicipal = inscricaoMunicipal;
        Email = email;
        Telefone = telefone;
        Celular = celular;
        Website = website;
        CEP = cep;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        DataAbertura = dataAbertura;
        CapitalSocial = capitalSocial;
        AtividadePrincipal = atividadePrincipal;
        RegimeTributario = regimeTributario;
        NomeResponsavel = nomeResponsavel;
        CPFResponsavel = cpfResponsavel;
        CargoResponsavel = cargoResponsavel;
        EmailResponsavel = emailResponsavel;
        TelefoneResponsavel = telefoneResponsavel;
    }
}

using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Interfaces;
using WMS.Domain.Entities;

namespace WMS.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompanyResponse?>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByTenantIdAsync(tenantId, cancellationToken);

        if (company == null)
        {
            return Result<CompanyResponse?>.Success(null);
        }

        var response = MapToResponse(company);
        return Result<CompanyResponse?>.Success(response);
    }

    public async Task<Result<CompanyResponse>> CreateAsync(
        Guid tenantId,
        CreateCompanyRequest request,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        // Validate: only one company per tenant
        var existingCompany = await _unitOfWork.Companies.GetByTenantIdAsync(tenantId, cancellationToken);

        if (existingCompany != null)
        {
            return Result<CompanyResponse>.Failure("Já existe uma empresa cadastrada para este tenant");
        }

        // Validate: CNPJ uniqueness
        var cnpjExists = await _unitOfWork.Companies.CNPJExistsAsync(request.CNPJ, cancellationToken);
        if (cnpjExists)
        {
            return Result<CompanyResponse>.Failure("CNPJ já cadastrado");
        }

        // Create company
        var company = new Company(
            tenantId: tenantId,
            razaoSocial: request.RazaoSocial,
            cnpj: request.CNPJ,
            email: request.Email,
            cep: request.CEP,
            logradouro: request.Logradouro,
            numero: request.Numero,
            bairro: request.Bairro,
            cidade: request.Cidade,
            estado: request.Estado,
            nomeResponsavel: request.NomeResponsavel,
            cpfResponsavel: request.CPFResponsavel,
            emailResponsavel: request.EmailResponsavel
        );

        // Update optional fields
        company.UpdateInfo(
            nomeFantasia: request.NomeFantasia,
            inscricaoEstadual: request.InscricaoEstadual,
            inscricaoMunicipal: request.InscricaoMunicipal,
            email: request.Email,
            telefone: request.Telefone,
            celular: request.Celular,
            website: request.Website,
            cep: request.CEP,
            logradouro: request.Logradouro,
            numero: request.Numero,
            complemento: request.Complemento,
            bairro: request.Bairro,
            cidade: request.Cidade,
            estado: request.Estado,
            dataAbertura: request.DataAbertura,
            capitalSocial: request.CapitalSocial,
            atividadePrincipal: request.AtividadePrincipal,
            regimeTributario: request.RegimeTributario,
            nomeResponsavel: request.NomeResponsavel,
            cpfResponsavel: request.CPFResponsavel,
            cargoResponsavel: request.CargoResponsavel,
            emailResponsavel: request.EmailResponsavel,
            telefoneResponsavel: request.TelefoneResponsavel
        );

        company.UpdateTimestamp(createdBy);
        await _unitOfWork.Companies.AddAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(company);
        return Result<CompanyResponse>.Success(response);
    }

    public async Task<Result<CompanyResponse>> UpdateAsync(
        Guid tenantId,
        UpdateCompanyRequest request,
        string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByTenantIdAsync(tenantId, cancellationToken);

        if (company == null)
        {
            return Result<CompanyResponse>.Failure("Empresa não encontrada");
        }

        // Update company info
        company.UpdateInfo(
            nomeFantasia: request.NomeFantasia ?? company.GetType().GetProperty("NomeFantasia")?.GetValue(company) as string,
            inscricaoEstadual: request.InscricaoEstadual ?? company.GetType().GetProperty("InscricaoEstadual")?.GetValue(company) as string,
            inscricaoMunicipal: request.InscricaoMunicipal ?? company.GetType().GetProperty("InscricaoMunicipal")?.GetValue(company) as string,
            email: request.Email ?? company.GetType().GetProperty("Email")?.GetValue(company) as string ?? "",
            telefone: request.Telefone ?? company.GetType().GetProperty("Telefone")?.GetValue(company) as string,
            celular: request.Celular ?? company.GetType().GetProperty("Celular")?.GetValue(company) as string,
            website: request.Website ?? company.GetType().GetProperty("Website")?.GetValue(company) as string,
            cep: request.CEP ?? company.GetType().GetProperty("CEP")?.GetValue(company) as string ?? "",
            logradouro: request.Logradouro ?? company.GetType().GetProperty("Logradouro")?.GetValue(company) as string ?? "",
            numero: request.Numero ?? company.GetType().GetProperty("Numero")?.GetValue(company) as string ?? "",
            complemento: request.Complemento ?? company.GetType().GetProperty("Complemento")?.GetValue(company) as string,
            bairro: request.Bairro ?? company.GetType().GetProperty("Bairro")?.GetValue(company) as string ?? "",
            cidade: request.Cidade ?? company.GetType().GetProperty("Cidade")?.GetValue(company) as string ?? "",
            estado: request.Estado ?? company.GetType().GetProperty("Estado")?.GetValue(company) as string ?? "",
            dataAbertura: request.DataAbertura ?? company.GetType().GetProperty("DataAbertura")?.GetValue(company) as DateTime?,
            capitalSocial: request.CapitalSocial ?? company.GetType().GetProperty("CapitalSocial")?.GetValue(company) as decimal?,
            atividadePrincipal: request.AtividadePrincipal ?? company.GetType().GetProperty("AtividadePrincipal")?.GetValue(company) as string,
            regimeTributario: request.RegimeTributario ?? company.GetType().GetProperty("RegimeTributario")?.GetValue(company) as string,
            nomeResponsavel: request.NomeResponsavel ?? company.GetType().GetProperty("NomeResponsavel")?.GetValue(company) as string ?? "",
            cpfResponsavel: request.CPFResponsavel ?? company.GetType().GetProperty("CPFResponsavel")?.GetValue(company) as string ?? "",
            cargoResponsavel: request.CargoResponsavel ?? company.GetType().GetProperty("CargoResponsavel")?.GetValue(company) as string,
            emailResponsavel: request.EmailResponsavel ?? company.GetType().GetProperty("EmailResponsavel")?.GetValue(company) as string ?? "",
            telefoneResponsavel: request.TelefoneResponsavel ?? company.GetType().GetProperty("TelefoneResponsavel")?.GetValue(company) as string
        );

        company.UpdateTimestamp(updatedBy);
        await _unitOfWork.Companies.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(company);
        return Result<CompanyResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid tenantId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByTenantIdAsync(tenantId, cancellationToken);

        if (company == null)
        {
            return Result.Failure("Empresa não encontrada");
        }

        company.MarkAsDeleted(deletedBy);
        await _unitOfWork.Companies.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static CompanyResponse MapToResponse(Company company)
    {
        // Use reflection to safely get private property values
        var type = company.GetType();
        
        return new CompanyResponse(
            Id: company.Id,
            RazaoSocial: type.GetProperty("RazaoSocial")?.GetValue(company) as string ?? "",
            NomeFantasia: type.GetProperty("NomeFantasia")?.GetValue(company) as string,
            CNPJ: type.GetProperty("CNPJ")?.GetValue(company) as string ?? "",
            InscricaoEstadual: type.GetProperty("InscricaoEstadual")?.GetValue(company) as string,
            InscricaoMunicipal: type.GetProperty("InscricaoMunicipal")?.GetValue(company) as string,
            Email: type.GetProperty("Email")?.GetValue(company) as string ?? "",
            Telefone: type.GetProperty("Telefone")?.GetValue(company) as string,
            Celular: type.GetProperty("Celular")?.GetValue(company) as string,
            Website: type.GetProperty("Website")?.GetValue(company) as string,
            CEP: type.GetProperty("CEP")?.GetValue(company) as string ?? "",
            Logradouro: type.GetProperty("Logradouro")?.GetValue(company) as string ?? "",
            Numero: type.GetProperty("Numero")?.GetValue(company) as string ?? "",
            Complemento: type.GetProperty("Complemento")?.GetValue(company) as string,
            Bairro: type.GetProperty("Bairro")?.GetValue(company) as string ?? "",
            Cidade: type.GetProperty("Cidade")?.GetValue(company) as string ?? "",
            Estado: type.GetProperty("Estado")?.GetValue(company) as string ?? "",
            DataAbertura: type.GetProperty("DataAbertura")?.GetValue(company) as DateTime?,
            CapitalSocial: type.GetProperty("CapitalSocial")?.GetValue(company) as decimal?,
            AtividadePrincipal: type.GetProperty("AtividadePrincipal")?.GetValue(company) as string,
            RegimeTributario: type.GetProperty("RegimeTributario")?.GetValue(company) as string,
            NomeResponsavel: type.GetProperty("NomeResponsavel")?.GetValue(company) as string ?? "",
            CPFResponsavel: type.GetProperty("CPFResponsavel")?.GetValue(company) as string ?? "",
            CargoResponsavel: type.GetProperty("CargoResponsavel")?.GetValue(company) as string,
            EmailResponsavel: type.GetProperty("EmailResponsavel")?.GetValue(company) as string ?? "",
            TelefoneResponsavel: type.GetProperty("TelefoneResponsavel")?.GetValue(company) as string,
            CreatedAt: company.CreatedAt,
            UpdatedAt: company.UpdatedAt
        );
    }
}

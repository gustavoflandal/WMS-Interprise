using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Interfaces;
using WMS.Domain.Entities;

namespace WMS.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<CustomerResponse>>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.GetByTenantIdAsync(tenantId, cancellationToken);
        var response = customers.Select(MapToResponse).ToList();
        return Result<List<CustomerResponse>>.Success(response);
    }

    public async Task<Result<CustomerResponse?>> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);

        if (customer == null || customer.IsDeleted || customer.TenantId != tenantId)
        {
            return Result<CustomerResponse?>.Success(null);
        }

        var response = MapToResponse(customer);
        return Result<CustomerResponse?>.Success(response);
    }

    public async Task<Result<CustomerResponse>> CreateAsync(
        Guid tenantId,
        CreateCustomerRequest request,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        // Validar: documento único por tenant (se fornecido)
        if (!string.IsNullOrEmpty(request.NumeroDocumento))
        {
            var documentoExists = await _unitOfWork.Customers.DocumentoExistsAsync(tenantId, request.NumeroDocumento, null, cancellationToken);
            if (documentoExists)
            {
                return Result<CustomerResponse>.Failure("Já existe um cliente com este documento");
            }
        }

        // Validar tipo
        if (request.Tipo != "PJ" && request.Tipo != "PF")
        {
            return Result<CustomerResponse>.Failure("Tipo deve ser 'PJ' ou 'PF'");
        }

        // Criar cliente
        var customer = new Customer(
            tenantId: tenantId,
            nome: request.Nome,
            tipo: request.Tipo,
            numeroDocumento: request.NumeroDocumento,
            email: request.Email,
            telefone: request.Telefone,
            criadoPor: userId
        );

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(customer);
        return Result<CustomerResponse>.Success(response);
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(
        Guid tenantId,
        Guid customerId,
        UpdateCustomerRequest request,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);

        if (customer == null || customer.IsDeleted || customer.TenantId != tenantId)
        {
            return Result<CustomerResponse>.Failure("Cliente não encontrado");
        }

        // Validar: documento único (se fornecido e diferente do atual)
        if (!string.IsNullOrEmpty(request.NumeroDocumento) && request.NumeroDocumento != customer.NumeroDocumento)
        {
            var documentoExists = await _unitOfWork.Customers.DocumentoExistsAsync(tenantId, request.NumeroDocumento, customerId, cancellationToken: cancellationToken);
            if (documentoExists)
            {
                return Result<CustomerResponse>.Failure("Já existe um cliente com este documento");
            }
        }

        // Validar tipo
        if (request.Tipo != "PJ" && request.Tipo != "PF")
        {
            return Result<CustomerResponse>.Failure("Tipo deve ser 'PJ' ou 'PF'");
        }

        // Atualizar informações
        customer.UpdateInfo(
            nome: request.Nome,
            tipo: request.Tipo,
            numeroDocumento: request.NumeroDocumento,
            email: request.Email,
            telefone: request.Telefone,
            atualizadoPor: userId
        );

        // Atualizar status se diferente
        if (customer.Status != (CustomerStatus)request.Status)
        {
            customer.UpdateStatus((CustomerStatus)request.Status, userId);
        }

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(customer);
        return Result<CustomerResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(
        Guid tenantId,
        Guid customerId,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);

        if (customer == null || customer.IsDeleted || customer.TenantId != tenantId)
        {
            return Result.Failure("Cliente não encontrado");
        }

        customer.MarkAsDeleted(userId?.ToString() ?? "System");

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            Id: customer.Id,
            Nome: customer.Nome,
            Tipo: customer.Tipo,
            NumeroDocumento: customer.NumeroDocumento,
            Email: customer.Email,
            Telefone: customer.Telefone,
            Status: (int)customer.Status,
            StatusDescricao: customer.Status switch
            {
                CustomerStatus.Ativo => "Ativo",
                CustomerStatus.Inativo => "Inativo",
                CustomerStatus.Bloqueado => "Bloqueado",
                _ => "Desconhecido"
            },
            CriadoPor: customer.CriadoPor,
            AtualizadoPor: customer.AtualizadoPor,
            CreatedAt: customer.CreatedAt,
            UpdatedAt: customer.UpdatedAt
        );
    }
}

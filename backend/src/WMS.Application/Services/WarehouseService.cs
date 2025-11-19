using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Interfaces;
using WMS.Domain.Entities;

namespace WMS.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<WarehouseResponse>>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByTenantIdAsync(tenantId, cancellationToken);
        var response = warehouses.Select(MapToResponse).ToList();
        return Result<IEnumerable<WarehouseResponse>>.Success(response);
    }

    public async Task<Result<WarehouseResponse?>> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);

        if (warehouse == null || warehouse.IsDeleted || warehouse.TenantId != tenantId)
        {
            return Result<WarehouseResponse?>.Success(null);
        }

        var response = MapToResponse(warehouse);
        return Result<WarehouseResponse?>.Success(response);
    }

    public async Task<Result<WarehouseResponse>> CreateAsync(
        Guid tenantId,
        CreateWarehouseRequest request,
        Guid? createdBy,
        CancellationToken cancellationToken = default)
    {
        // Validate: código uniqueness per tenant
        var codeExists = await _unitOfWork.Warehouses.CodeExistsAsync(tenantId, request.Codigo, cancellationToken);
        if (codeExists)
        {
            return Result<WarehouseResponse>.Failure("Já existe um armazém com este código");
        }

        // Create warehouse
        var warehouse = new Warehouse(
            tenantId: tenantId,
            nome: request.Nome,
            codigo: request.Codigo,
            descricao: request.Descricao,
            criadoPor: createdBy
        );

        // Update optional fields
        warehouse.UpdateInfo(
            nome: request.Nome,
            descricao: request.Descricao,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            cep: request.CEP,
            pais: request.Pais ?? "BRA",
            latitude: request.Latitude,
            longitude: request.Longitude,
            totalPosicoes: request.TotalPosicoes,
            capacidadePesoTotal: request.CapacidadePesoTotal,
            horarioAbertura: request.HorarioAbertura,
            horarioFechamento: request.HorarioFechamento,
            maxTrabalhadores: request.MaxTrabalhadores,
            atualizadoPor: createdBy
        );

        await _unitOfWork.Warehouses.AddAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(warehouse);
        return Result<WarehouseResponse>.Success(response);
    }

    public async Task<Result<WarehouseResponse>> UpdateAsync(
        Guid tenantId,
        Guid id,
        UpdateWarehouseRequest request,
        Guid? updatedBy,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);

        if (warehouse == null || warehouse.IsDeleted || warehouse.TenantId != tenantId)
        {
            return Result<WarehouseResponse>.Failure("Armazém não encontrado");
        }

        // Update info
        warehouse.UpdateInfo(
            nome: request.Nome,
            descricao: request.Descricao,
            endereco: request.Endereco,
            cidade: request.Cidade,
            estado: request.Estado,
            cep: request.CEP,
            pais: request.Pais ?? "BRA",
            latitude: request.Latitude,
            longitude: request.Longitude,
            totalPosicoes: request.TotalPosicoes,
            capacidadePesoTotal: request.CapacidadePesoTotal,
            horarioAbertura: request.HorarioAbertura,
            horarioFechamento: request.HorarioFechamento,
            maxTrabalhadores: request.MaxTrabalhadores,
            atualizadoPor: updatedBy
        );

        // Update status
        var status = (WarehouseStatus)request.Status;
        warehouse.UpdateStatus(status, updatedBy);

        await _unitOfWork.Warehouses.UpdateAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(warehouse);
        return Result<WarehouseResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid tenantId, Guid id, Guid? deletedBy, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);

        if (warehouse == null || warehouse.IsDeleted || warehouse.TenantId != tenantId)
        {
            return Result.Failure("Armazém não encontrado");
        }

        // Soft delete
        warehouse.MarkAsDeleted(deletedBy?.ToString() ?? "System");
        await _unitOfWork.Warehouses.UpdateAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static WarehouseResponse MapToResponse(Warehouse warehouse)
    {
        var statusDescricao = warehouse.Status switch
        {
            WarehouseStatus.Ativo => "Ativo",
            WarehouseStatus.Inativo => "Inativo",
            WarehouseStatus.EmManutencao => "Em Manutenção",
            _ => "Desconhecido"
        };

        return new WarehouseResponse(
            Id: warehouse.Id,
            Nome: warehouse.Nome,
            Codigo: warehouse.Codigo,
            Descricao: warehouse.Descricao,
            Endereco: warehouse.Endereco,
            Cidade: warehouse.Cidade,
            Estado: warehouse.Estado,
            CEP: warehouse.CEP,
            Pais: warehouse.Pais,
            Latitude: warehouse.Latitude,
            Longitude: warehouse.Longitude,
            TotalPosicoes: warehouse.TotalPosicoes,
            CapacidadePesoTotal: warehouse.CapacidadePesoTotal,
            HorarioAbertura: warehouse.HorarioAbertura,
            HorarioFechamento: warehouse.HorarioFechamento,
            MaxTrabalhadores: warehouse.MaxTrabalhadores,
            Status: (int)warehouse.Status,
            StatusDescricao: statusDescricao,
            CriadoPor: warehouse.CriadoPor,
            AtualizadoPor: warehouse.AtualizadoPor,
            CreatedAt: warehouse.CreatedAt,
            UpdatedAt: warehouse.UpdatedAt
        );
    }
}

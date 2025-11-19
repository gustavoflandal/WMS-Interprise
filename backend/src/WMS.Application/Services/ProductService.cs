using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

/// <summary>
/// Implementação do serviço de negócio para Produtos
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<ProductResponse>.Failure("Produto não encontrado");

        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<ProductResponse>> GetBySkuAsync(string sku, int tenantId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return Result<ProductResponse>.Failure("SKU é obrigatório");

        var product = await _unitOfWork.Products.GetBySkuAsync(sku, tenantId);
        if (product == null)
            return Result<ProductResponse>.Failure("Produto com este SKU não encontrado");

        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetActiveAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetActiveAsync(tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetByCategoryAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        if (categoryId < 0 || categoryId > 7)
            return Result<IEnumerable<ProductResponse>>.Failure("Categoria inválida");

        var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId, tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetByABCClassificationAsync(int classification, int tenantId, CancellationToken cancellationToken = default)
    {
        if (classification < 0 || classification > 2)
            return Result<IEnumerable<ProductResponse>>.Failure("Classificação ABC inválida");

        var products = await _unitOfWork.Products.GetByABCClassificationAsync(classification, tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetRequiringLotTrackingAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetRequiringLotTrackingAsync(tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetRequiringSerialNumberAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetRequiringSerialNumberAsync(tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetPerishableAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetPerishableAsync(tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetRequiringTemperatureControlAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetRequiringTemperatureControlAsync(tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ProductResponse>>> SearchAsync(string searchTerm, int tenantId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<IEnumerable<ProductResponse>>.Failure("Termo de busca é obrigatório");

        var products = await _unitOfWork.Products.SearchAsync(searchTerm, tenantId);
        var responses = products.Select(MapToResponse);
        return Result<IEnumerable<ProductResponse>>.Success(responses);
    }

    public async Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, int tenantId, string createdBy, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.SkuExistsAsync(request.Sku, tenantId))
            return Result<ProductResponse>.Failure("Já existe um produto com este SKU");

        var product = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            Category = (ProductCategory)request.Category,
            Type = (ProductType)request.Type,
            UnitWeight = request.UnitWeight,
            UnitVolume = request.UnitVolume,
            DefaultStorageZone = (StorageZone)request.DefaultStorageZone,
            RequiresLotTracking = request.RequiresLotTracking,
            RequiresSerialNumber = request.RequiresSerialNumber,
            ShelfLifeDays = request.ShelfLifeDays,
            MinStorageTemperature = request.MinStorageTemperature,
            MaxStorageTemperature = request.MaxStorageTemperature,
            MinStorageHumidity = request.MinStorageHumidity,
            MaxStorageHumidity = request.MaxStorageHumidity,
            IsFlammable = request.IsFlammable,
            IsDangerous = request.IsDangerous,
            IsPharmaceutical = request.IsPharmaceutical,
            ABCClassification = request.ABCClassification.HasValue ? (ABCClassification)request.ABCClassification.Value : null,
            UnitCost = request.UnitCost,
            UnitPrice = request.UnitPrice,
            TenantId = tenantId,
            IsActive = true
        };

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    public async Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, string updatedBy, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<ProductResponse>.Failure("Produto não encontrado");

        if (!string.IsNullOrWhiteSpace(request.Name))
            product.Name = request.Name;

        if (request.Category.HasValue)
            product.Category = (ProductCategory)request.Category.Value;

        if (request.Type.HasValue)
            product.Type = (ProductType)request.Type.Value;

        if (request.UnitWeight.HasValue)
            product.UnitWeight = request.UnitWeight.Value;

        if (request.UnitVolume.HasValue)
            product.UnitVolume = request.UnitVolume.Value;

        if (request.DefaultStorageZone.HasValue)
            product.DefaultStorageZone = (StorageZone)request.DefaultStorageZone.Value;

        if (request.RequiresLotTracking.HasValue)
            product.RequiresLotTracking = request.RequiresLotTracking.Value;

        if (request.RequiresSerialNumber.HasValue)
            product.RequiresSerialNumber = request.RequiresSerialNumber.Value;

        if (request.ShelfLifeDays.HasValue)
            product.ShelfLifeDays = request.ShelfLifeDays.Value;

        if (request.MinStorageTemperature.HasValue)
            product.MinStorageTemperature = request.MinStorageTemperature.Value;

        if (request.MaxStorageTemperature.HasValue)
            product.MaxStorageTemperature = request.MaxStorageTemperature.Value;

        if (request.MinStorageHumidity.HasValue)
            product.MinStorageHumidity = request.MinStorageHumidity.Value;

        if (request.MaxStorageHumidity.HasValue)
            product.MaxStorageHumidity = request.MaxStorageHumidity.Value;

        if (request.IsFlammable.HasValue)
            product.IsFlammable = request.IsFlammable.Value;

        if (request.IsDangerous.HasValue)
            product.IsDangerous = request.IsDangerous.Value;

        if (request.IsPharmaceutical.HasValue)
            product.IsPharmaceutical = request.IsPharmaceutical.Value;

        if (request.ABCClassification.HasValue)
            product.ABCClassification = (ABCClassification)request.ABCClassification.Value;

        if (request.UnitCost.HasValue)
            product.UnitCost = request.UnitCost.Value;

        if (request.UnitPrice.HasValue)
            product.UnitPrice = request.UnitPrice.Value;

        if (request.IsActive.HasValue)
            product.IsActive = request.IsActive.Value;

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    public async Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result.Failure("Produto não encontrado");

        await _unitOfWork.Products.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<bool>> SkuExistsAsync(string sku, int tenantId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return Result<bool>.Failure("SKU é obrigatório");

        var exists = await _unitOfWork.Products.SkuExistsAsync(sku, tenantId);
        return Result<bool>.Success(exists);
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Category = (int)product.Category,
            Type = (int)product.Type,
            UnitWeight = product.UnitWeight,
            UnitVolume = product.UnitVolume,
            DefaultStorageZone = (int)product.DefaultStorageZone,
            RequiresLotTracking = product.RequiresLotTracking,
            RequiresSerialNumber = product.RequiresSerialNumber,
            ShelfLifeDays = product.ShelfLifeDays,
            MinStorageTemperature = product.MinStorageTemperature,
            MaxStorageTemperature = product.MaxStorageTemperature,
            MinStorageHumidity = product.MinStorageHumidity,
            MaxStorageHumidity = product.MaxStorageHumidity,
            IsFlammable = product.IsFlammable,
            IsDangerous = product.IsDangerous,
            IsPharmaceutical = product.IsPharmaceutical,
            ABCClassification = (int?)product.ABCClassification,
            UnitCost = product.UnitCost,
            UnitPrice = product.UnitPrice,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt ?? DateTime.UtcNow
        };
    }
}

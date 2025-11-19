using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Interface para o repositório de Produtos (SKUs)
    /// Define operações de acesso a dados para produtos
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Obtém um produto pelo seu código SKU
        /// </summary>
        Task<Product?> GetBySkuAsync(string sku, int tenantId);

        /// <summary>
        /// Verifica se um SKU já existe para um tenant
        /// </summary>
        Task<bool> SkuExistsAsync(string sku, int tenantId);

        /// <summary>
        /// Obtém todos os produtos ativos de um tenant
        /// </summary>
        Task<IEnumerable<Product>> GetActiveAsync(int tenantId);

        /// <summary>
        /// Obtém produtos por categoria
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId);

        /// <summary>
        /// Obtém produtos com classificação ABC específica
        /// </summary>
        Task<IEnumerable<Product>> GetByABCClassificationAsync(int classification, int tenantId);

        /// <summary>
        /// Obtém produtos que requerem rastreamento de lote
        /// </summary>
        Task<IEnumerable<Product>> GetRequiringLotTrackingAsync(int tenantId);

        /// <summary>
        /// Obtém produtos que requerem rastreamento de série
        /// </summary>
        Task<IEnumerable<Product>> GetRequiringSerialNumberAsync(int tenantId);

        /// <summary>
        /// Obtém produtos perecíveis (com shelf life)
        /// </summary>
        Task<IEnumerable<Product>> GetPerishableAsync(int tenantId);

        /// <summary>
        /// Pesquisa produtos por nome ou SKU
        /// </summary>
        Task<IEnumerable<Product>> SearchAsync(string searchTerm, int tenantId);

        /// <summary>
        /// Obtém produtos que requerem controle de temperatura
        /// </summary>
        Task<IEnumerable<Product>> GetRequiringTemperatureControlAsync(int tenantId);

        /// <summary>
        /// Obtém produtos perigosos (inflamáveis, tóxicos, etc)
        /// </summary>
        Task<IEnumerable<Product>> GetDangerousAsync(int tenantId);

        /// <summary>
        /// Obtém produtos farmacêuticos
        /// </summary>
        Task<IEnumerable<Product>> GetPharmaceuticalAsync(int tenantId);
    }
}

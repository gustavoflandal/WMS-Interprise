using WMS.Domain.Enums;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um SKU (Stock Keeping Unit) / Produto no sistema
    /// Contém informações sobre produtos que podem ser armazenados nos armazéns
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Código único do produto (SKU)
        /// Exemplo: SKU-001-ABC
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Nome do produto
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do produto
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Categoria do produto (Seco, Refrigerado, Congelado, Perecível, etc)
        /// </summary>
        public ProductCategory Category { get; set; }

        /// <summary>
        /// Tipo do produto (Commodity, Fracionável, Frágil, etc)
        /// </summary>
        public ProductType Type { get; set; }

        /// <summary>
        /// Peso unitário do produto em kg
        /// </summary>
        public decimal UnitWeight { get; set; }

        /// <summary>
        /// Volume unitário do produto em m³
        /// </summary>
        public decimal UnitVolume { get; set; }

        /// <summary>
        /// Zona de armazenamento padrão para este produto
        /// (Picking, Reserva, Cross-Dock, Quarentena)
        /// </summary>
        public StorageZone DefaultStorageZone { get; set; }

        /// <summary>
        /// Indica se o produto requer rastreamento de lote
        /// </summary>
        public bool RequiresLotTracking { get; set; }

        /// <summary>
        /// Indica se o produto requer rastreamento de número de série
        /// </summary>
        public bool RequiresSerialNumber { get; set; }

        /// <summary>
        /// Período de vida útil do produto em dias (shelf life)
        /// Usado para produtos perecíveis
        /// </summary>
        public int? ShelfLifeDays { get; set; }

        /// <summary>
        /// Indica se o produto está ativo no sistema
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ID do tenant proprietário deste produto
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Temperatura mínima recomendada para armazenamento (em Celsius)
        /// Usado para produtos refrigerados/congelados
        /// </summary>
        public decimal? MinStorageTemperature { get; set; }

        /// <summary>
        /// Temperatura máxima recomendada para armazenamento (em Celsius)
        /// Usado para produtos refrigerados/congelados
        /// </summary>
        public decimal? MaxStorageTemperature { get; set; }

        /// <summary>
        /// Umidade relativa mínima recomendada (em percentual)
        /// </summary>
        public decimal? MinStorageHumidity { get; set; }

        /// <summary>
        /// Umidade relativa máxima recomendada (em percentual)
        /// </summary>
        public decimal? MaxStorageHumidity { get; set; }

        /// <summary>
        /// Indica se o produto é inflamável
        /// </summary>
        public bool IsFlammable { get; set; }

        /// <summary>
        /// Indica se o produto é tóxico ou perigoso
        /// </summary>
        public bool IsDangerous { get; set; }

        /// <summary>
        /// Indica se o produto é um medicamento (requer rastreabilidade especial)
        /// </summary>
        public bool IsPharmaceutical { get; set; }

        /// <summary>
        /// Classificação ABC do produto (baseada em giro/valor)
        /// A = Alto giro, B = Médio giro, C = Baixo giro
        /// </summary>
        public ABCClassification? ABCClassification { get; set; }

        /// <summary>
        /// Custo unitário do produto (em reais)
        /// </summary>
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Preço de venda unitário (em reais)
        /// </summary>
        public decimal? UnitPrice { get; set; }
    }
}

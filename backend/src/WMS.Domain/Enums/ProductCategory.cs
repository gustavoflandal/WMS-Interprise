namespace WMS.Domain.Enums
{
    /// <summary>
    /// Enumeração das categorias de produtos suportadas pelo WMS
    /// Determina como e onde o produto pode ser armazenado
    /// </summary>
    public enum ProductCategory
    {
        /// <summary>
        /// Produtos secos, sem restrição de temperatura
        /// Exemplos: alimentos secos, têxteis, etc
        /// </summary>
        Dry = 1,

        /// <summary>
        /// Produtos refrigerados (2-8°C)
        /// Exemplos: laticínios, carnes, etc
        /// </summary>
        Refrigerated = 2,

        /// <summary>
        /// Produtos congelados (-18°C ou mais frio)
        /// Exemplos: alimentos congelados, sorvete, etc
        /// </summary>
        Frozen = 3,

        /// <summary>
        /// Produtos perecíveis com data de validade crítica
        /// Exemplos: frutas, vegetais, produtos com data de vencimento
        /// </summary>
        Perishable = 4,

        /// <summary>
        /// Produtos controlados que necessitam rastreabilidade especial
        /// Exemplos: medicamentos, cosméticos controlados, bebidas alcoólicas
        /// </summary>
        Controlled = 5,

        /// <summary>
        /// Produtos de grande volume/paletizados
        /// Exemplos: bebidas em caixas, produtos em big bags
        /// </summary>
        BulkVolume = 6,

        /// <summary>
        /// Produtos de pequeno volume/fracionados
        /// Exemplos: caixas pequenas, unidades individuais
        /// </summary>
        SmallVolume = 7,

        /// <summary>
        /// Produtos de alto valor que requerem segurança especial
        /// Exemplos: eletrônicos, joias, componentes de alto custo
        /// </summary>
        HighValue = 8
    }
}

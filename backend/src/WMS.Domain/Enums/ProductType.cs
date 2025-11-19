namespace WMS.Domain.Enums
{
    /// <summary>
    /// Enumeração dos tipos de produtos suportados
    /// Descreve características físicas e de manuseio do produto
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// Produto genérico/commodity
        /// Pode ser armazenado conforme padrão
        /// </summary>
        Commodity = 1,

        /// <summary>
        /// Produto fracionável
        /// Pode ser dividido em menores quantidades durante picking
        /// </summary>
        Fractionable = 2,

        /// <summary>
        /// Produto frágil
        /// Requer cuidado especial no manuseio e armazenamento
        /// </summary>
        Fragile = 3,

        /// <summary>
        /// Produto pesado
        /// Requer estrutura de armazenamento adequada para carga
        /// </summary>
        Heavy = 4,

        /// <summary>
        /// Produto líquido
        /// Requer contenção contra derramamentos
        /// </summary>
        Liquid = 5,

        /// <summary>
        /// Produto gasoso
        /// Requer armazenamento pressurizado/especial
        /// </summary>
        Gaseous = 6,

        /// <summary>
        /// Produto que ocupa muito espaço relativamente ao peso
        /// Exemplo: colchões, móveis
        /// </summary>
        Bulky = 7,

        /// <summary>
        /// Produto que requer separação/isolamento especial
        /// Pode ser alergênico, tóxico ou ter restrições de proximidade
        /// </summary>
        IsolationRequired = 8
    }
}

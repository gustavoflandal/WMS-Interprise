namespace WMS.Domain.Enums
{
    /// <summary>
    /// Enumeração das zonas de armazenamento disponíveis em um armazém
    /// Define regiões físicas com características e finalidades específicas
    /// </summary>
    public enum StorageZone
    {
        /// <summary>
        /// Zona de picking - Produtos de alto giro com fácil acesso
        /// Otimizada para rapidez de coleta
        /// </summary>
        Picking = 1,

        /// <summary>
        /// Zona de reserva - Produtos de médio/baixo giro
        /// Armazenamento de longo prazo
        /// </summary>
        Reserve = 2,

        /// <summary>
        /// Zona de cross-dock - Produtos em trânsito
        /// Sem armazenagem prolongada, apenas consolidação e redistribuição
        /// </summary>
        CrossDock = 3,

        /// <summary>
        /// Zona de quarentena - Produtos pendentes de conferência
        /// Produtos aguardando inspeção de qualidade ou documentação
        /// </summary>
        Quarantine = 4,

        /// <summary>
        /// Zona de danos/devoluções
        /// Produtos danificados ou devolvidos pelo cliente
        /// </summary>
        Damage = 5,

        /// <summary>
        /// Zona refrigerada - Temperatura controlada 2-8°C
        /// Para produtos que requerem refrigeração
        /// </summary>
        Refrigerated = 6,

        /// <summary>
        /// Zona congelada - Temperatura controlada -18°C ou mais fria
        /// Para produtos congelados
        /// </summary>
        Frozen = 7,

        /// <summary>
        /// Zona de produtos controlados/perigosos
        /// Medicamentos, bebidas alcoólicas, produtos perigosos
        /// </summary>
        Controlled = 8,

        /// <summary>
        /// Zona de consolidação - Área de montagem de pedidos
        /// Antes do packing e expedição
        /// </summary>
        Consolidation = 9,

        /// <summary>
        /// Zona de packing - Embalagem e fechamento de pedidos
        /// </summary>
        Packing = 10
    }
}

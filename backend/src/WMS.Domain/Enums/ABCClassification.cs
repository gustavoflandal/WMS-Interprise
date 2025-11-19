namespace WMS.Domain.Enums
{
    /// <summary>
    /// Classificação ABC de produtos baseada em giro/valor
    /// Utilizada para otimizar alocação em armazém
    /// </summary>
    public enum ABCClassification
    {
        /// <summary>
        /// Classe A - Alto giro e/ou alto valor
        /// Representa ~20% dos produtos e ~80% do faturamento
        /// Deve ser armazenado em local de fácil acesso (Picking Zone)
        /// </summary>
        A = 1,

        /// <summary>
        /// Classe B - Médio giro e/ou médio valor
        /// Representa ~30% dos produtos e ~15% do faturamento
        /// Armazenado em zona intermediária
        /// </summary>
        B = 2,

        /// <summary>
        /// Classe C - Baixo giro e/ou baixo valor
        /// Representa ~50% dos produtos e ~5% do faturamento
        /// Pode ser armazenado em locais menos acessíveis (Reserve Zone)
        /// </summary>
        C = 3
    }
}

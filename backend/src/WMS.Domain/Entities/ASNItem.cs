namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um item individual dentro de uma ASN
    /// Cada linha da ASN descreve um SKU específico que será recebido
    /// </summary>
    public class ASNItem : BaseEntity
    {
        /// <summary>
        /// ID da ASN à qual este item pertence
        /// </summary>
        public int AsnId { get; set; }

        /// <summary>
        /// ID do produto (SKU) que será recebido
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Quantidade esperada a receber deste produto
        /// </summary>
        public int ExpectedQuantity { get; set; }

        /// <summary>
        /// Quantidade já recebida deste produto
        /// </summary>
        public int ReceivedQuantity { get; set; }

        /// <summary>
        /// Unidade de medida
        /// Exemplos: UN (Unidade), KG (Quilograma), M (Metro), L (Litro)
        /// </summary>
        public string Unit { get; set; } = "UN";

        /// <summary>
        /// Peso esperado deste item em kg
        /// </summary>
        public decimal? ExpectedWeight { get; set; }

        /// <summary>
        /// Volume esperado deste item em m³
        /// </summary>
        public decimal? ExpectedVolume { get; set; }

        /// <summary>
        /// Data de validade do lote
        /// Usado para produtos perecíveis
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Número do lote/série
        /// Utilizado para rastreabilidade
        /// </summary>
        public string? LotNumber { get; set; }

        /// <summary>
        /// Número de série (se aplicável)
        /// Para produtos que requerem rastreamento individual
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Indica se este item foi completamente recebido e conferido
        /// </summary>
        public bool IsConformed { get; set; }

        /// <summary>
        /// Qualidade do item conforme classificação
        /// </summary>
        public ItemQualityStatus QualityStatus { get; set; } = ItemQualityStatus.Pending;

        /// <summary>
        /// Motivo de rejeição (se aplicável)
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Observações sobre este item
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Valor unitário do item (em reais)
        /// </summary>
        public decimal? UnitValue { get; set; }

        /// <summary>
        /// Indica se o item requer inspeção de qualidade especial
        /// </summary>
        public bool RequiresQualityInspection { get; set; }

        /// <summary>
        /// ID do usuário que recebeu este item
        /// </summary>
        public int? ReceivedByUserId { get; set; }

        /// <summary>
        /// Data/hora de recebimento
        /// </summary>
        public DateTime? ReceivedAt { get; set; }

        /// <summary>
        /// Variância de quantidade
        /// Diferença entre esperado e recebido
        /// </summary>
        public int QuantityVariance => ReceivedQuantity - ExpectedQuantity;

        /// <summary>
        /// Percentual de variância
        /// </summary>
        public decimal VariancePercentage =>
            ExpectedQuantity == 0 ? 0 : (decimal)(QuantityVariance * 100) / ExpectedQuantity;
    }

    /// <summary>
    /// Enumeração do status de qualidade de um item recebido
    /// </summary>
    public enum ItemQualityStatus
    {
        /// <summary>
        /// Pendente de conferência/inspeção
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Aceito sem ressalvas
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// Aceito com ressalvas/ajustes
        /// </summary>
        AcceptedWithRemarks = 3,

        /// <summary>
        /// Rejeitado
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Devolvido ao fornecedor
        /// </summary>
        ReturnedToSupplier = 5,

        /// <summary>
        /// Em quarentena aguardando inspeção adicional
        /// </summary>
        InQuarantine = 6
    }
}

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um item específico dentro de um recebimento
    /// Detalha o que foi recebido, onde foi armazenado e seu status de qualidade
    /// </summary>
    public class ReceiptItem : BaseEntity
    {
        /// <summary>
        /// ID do recebimento ao qual este item pertence
        /// </summary>
        public int ReceiptDocumentationId { get; set; }

        /// <summary>
        /// ID do produto (SKU) que foi recebido
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Quantidade de unidades recebidas
        /// </summary>
        public int QuantityReceived { get; set; }

        /// <summary>
        /// Quantidade esperada do ASN (para comparação)
        /// </summary>
        public int? QuantityExpected { get; set; }

        /// <summary>
        /// Unidade de medida
        /// </summary>
        public string Unit { get; set; } = "UN";

        /// <summary>
        /// Peso total recebido deste item em kg
        /// </summary>
        public decimal ActualWeight { get; set; }

        /// <summary>
        /// Volume total deste item em m³
        /// </summary>
        public decimal? ActualVolume { get; set; }

        /// <summary>
        /// Número do lote/série do produto recebido
        /// </summary>
        public string? LotNumber { get; set; }

        /// <summary>
        /// Data de validade/vencimento do lote
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Números de série dos itens (se rastreados individualmente)
        /// Separados por vírgula
        /// </summary>
        public string? SerialNumbers { get; set; }

        /// <summary>
        /// ID da localização onde este item foi armazenado
        /// </summary>
        public int StorageLocationId { get; set; }

        /// <summary>
        /// Status de qualidade deste item conforme recebimento
        /// </summary>
        public ReceiptItemQualityStatus QualityStatus { get; set; }

        /// <summary>
        /// Motivo da rejeição (se o item foi rejeitado)
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Observações sobre este item
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Valor unitário declarado (em reais)
        /// </summary>
        public decimal? DeclaredUnitValue { get; set; }

        /// <summary>
        /// Valor total deste item
        /// </summary>
        public decimal? TotalValue => DeclaredUnitValue.HasValue ? DeclaredUnitValue.Value * QuantityReceived : null;

        /// <summary>
        /// Indicador de que o item foi inspecionado
        /// </summary>
        public bool IsInspected { get; set; }

        /// <summary>
        /// ID do usuário que inspecionou este item
        /// </summary>
        public int? InspectedByUserId { get; set; }

        /// <summary>
        /// Data/hora da inspeção
        /// </summary>
        public DateTime? InspectedAt { get; set; }

        /// <summary>
        /// Temperatura do produto no momento do recebimento
        /// </summary>
        public decimal? TemperatureAtReceipt { get; set; }

        /// <summary>
        /// Umidade relativa no momento do recebimento
        /// </summary>
        public decimal? HumidityAtReceipt { get; set; }

        /// <summary>
        /// Foto/evidência do recebimento (URL ou caminho)
        /// </summary>
        public string? EvidencePhotoUrl { get; set; }

        /// <summary>
        /// Indica se há discrepância neste item
        /// </summary>
        public bool HasDiscrepancy => QuantityExpected.HasValue && QuantityReceived != QuantityExpected.Value;

        /// <summary>
        /// Quantidade de discrepância
        /// </summary>
        public int DiscrepancyQuantity => QuantityExpected.HasValue ? QuantityReceived - QuantityExpected.Value : 0;

        /// <summary>
        /// Percentual de discrepância
        /// </summary>
        public decimal DiscrepancyPercentage =>
            QuantityExpected.HasValue && QuantityExpected.Value > 0
                ? (decimal)(DiscrepancyQuantity * 100) / QuantityExpected.Value
                : 0;
    }

    /// <summary>
    /// Enumeração do status de qualidade de um item recebido
    /// </summary>
    public enum ReceiptItemQualityStatus
    {
        /// <summary>
        /// Aceito sem ressalvas
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// Aceito com ressalvas/ajustes menores
        /// </summary>
        AcceptedWithRemarks = 2,

        /// <summary>
        /// Rejeitado - não aceito
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Aceito parcialmente (quantidade reduzida)
        /// </summary>
        PartiallyAccepted = 4,

        /// <summary>
        /// Devolvido ao fornecedor
        /// </summary>
        ReturnedToSupplier = 5,

        /// <summary>
        /// Em inspeção/quarentena
        /// </summary>
        UnderInspection = 6,

        /// <summary>
        /// Pendente de inspeção
        /// </summary>
        PendingInspection = 7
    }
}

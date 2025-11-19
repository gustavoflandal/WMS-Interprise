namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa a documentação e registro de um recebimento de carga
    /// Consolida os dados de uma ASN com os itens efetivamente recebidos
    /// </summary>
    public class ReceiptDocumentation : BaseEntity
    {
        /// <summary>
        /// ID do armazém que recebeu a carga
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// ID da ASN associada (opcional - recebimentos podem ser sem ASN)
        /// </summary>
        public int? AsnId { get; set; }

        /// <summary>
        /// Número único do recebimento
        /// Identificador principal do documento
        /// </summary>
        public string ReceiptNumber { get; set; } = string.Empty;

        /// <summary>
        /// Data do recebimento
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// Status atual do recebimento
        /// </summary>
        public ReceiptStatus Status { get; set; }

        /// <summary>
        /// Quantidade total de itens recebidos
        /// </summary>
        public int TotalItemsReceived { get; set; }

        /// <summary>
        /// Peso total recebido em kg
        /// </summary>
        public decimal TotalWeight { get; set; }

        /// <summary>
        /// Volume total recebido em m³
        /// </summary>
        public decimal? TotalVolume { get; set; }

        /// <summary>
        /// ID do operador/funcionário que realizou o recebimento
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// ID do supervisor que conferiu o recebimento (se aplicável)
        /// </summary>
        public int? SupervisorId { get; set; }

        /// <summary>
        /// Número da Nota Fiscal associada
        /// </summary>
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Referência externa (número de pedido, transferência, etc)
        /// </summary>
        public string? ExternalReference { get; set; }

        /// <summary>
        /// Observações gerais sobre o recebimento
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Data/hora em que o recebimento foi confirmado
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// ID do usuário que confirmou o recebimento
        /// </summary>
        public int? ConfirmedByUserId { get; set; }

        /// <summary>
        /// Data/hora em que o recebimento foi fechado
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// ID do usuário que fechou o recebimento
        /// </summary>
        public int? ClosedByUserId { get; set; }

        /// <summary>
        /// Quantidade de itens com discrepância (maior ou menor que esperado)
        /// </summary>
        public int ItemsWithDiscrepancy { get; set; }

        /// <summary>
        /// Quantidade de itens rejeitados
        /// </summary>
        public int RejectedItems { get; set; }

        /// <summary>
        /// Indica se há discrepâncias documentadas
        /// </summary>
        public bool HasDiscrepancies { get; set; }

        /// <summary>
        /// Tempo total de recebimento em minutos
        /// Utilizado para métricas de produtividade
        /// </summary>
        public int? TotalReceiptTimeMinutes { get; set; }

        /// <summary>
        /// Tipo de recebimento
        /// Normal, Transferência entre armazéns, Devolução, etc
        /// </summary>
        public ReceiptType ReceiptType { get; set; } = ReceiptType.Purchase;

        /// <summary>
        /// ID do fornecedor/origem
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// Transportadora utilizada
        /// </summary>
        public string? Carrier { get; set; }

        /// <summary>
        /// Número de rastreamento da transportadora
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Coleção de itens recebidos
        /// </summary>
        public List<ReceiptItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Enumeração dos status possíveis de um recebimento
    /// </summary>
    public enum ReceiptStatus
    {
        /// <summary>
        /// Recebimento em rascunho (ainda não iniciado)
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Recebimento em andamento
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Recebimento confirmado
        /// </summary>
        Confirmed = 3,

        /// <summary>
        /// Recebimento completo e fechado
        /// </summary>
        Closed = 4,

        /// <summary>
        /// Recebimento cancelado
        /// </summary>
        Cancelled = 5,

        /// <summary>
        /// Recebimento em quarentena (aguardando inspeção)
        /// </summary>
        OnHold = 6
    }

    /// <summary>
    /// Enumeração dos tipos de recebimento
    /// </summary>
    public enum ReceiptType
    {
        /// <summary>
        /// Recebimento de compra
        /// </summary>
        Purchase = 1,

        /// <summary>
        /// Transferência entre armazéns
        /// </summary>
        TransferBetweenWarehouses = 2,

        /// <summary>
        /// Devolução de cliente
        /// </summary>
        CustomerReturn = 3,

        /// <summary>
        /// Devolução ao fornecedor
        /// </summary>
        SupplierReturn = 4,

        /// <summary>
        /// Reabastecimento interno
        /// </summary>
        InternalReplenishment = 5,

        /// <summary>
        /// Recebimento de ajuste/correção
        /// </summary>
        Adjustment = 6
    }
}

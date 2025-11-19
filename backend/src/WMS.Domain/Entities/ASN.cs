namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um Aviso de Remessa Antecipado (ASN - Advance Shipping Notice)
    /// Notificação prévia de uma carga que será recebida no armazém
    /// </summary>
    public class ASN : BaseEntity
    {
        /// <summary>
        /// ID do armazém que receberá esta carga
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Número único da ASN
        /// Identificador único da notificação
        /// </summary>
        public string AsnNumber { get; set; } = string.Empty;

        /// <summary>
        /// ID do fornecedor/transportador que enviará a carga
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// Número da Nota Fiscal associada à carga
        /// </summary>
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Data prevista de chegada da carga
        /// </summary>
        public DateTime ScheduledArrivalDate { get; set; }

        /// <summary>
        /// Data efetiva de chegada da carga
        /// Null até o recebimento
        /// </summary>
        public DateTime? ActualArrivalDate { get; set; }

        /// <summary>
        /// Status atual da ASN
        /// </summary>
        public ASNStatus Status { get; set; }

        /// <summary>
        /// Quantidade esperada de itens/linhas nesta carga
        /// </summary>
        public int ExpectedItemCount { get; set; }

        /// <summary>
        /// Peso total esperado da carga em kg
        /// </summary>
        public decimal ExpectedTotalWeight { get; set; }

        /// <summary>
        /// Volume total esperado da carga em m³
        /// </summary>
        public decimal? ExpectedTotalVolume { get; set; }

        /// <summary>
        /// Quantidade de itens já recebidos
        /// </summary>
        public int ReceivedItemCount { get; set; }

        /// <summary>
        /// Observações/notas sobre a carga
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// ID do usuário que criou esta ASN
        /// </summary>
        public int? CreatedByUserId { get; set; }

        /// <summary>
        /// ID do armazém de origem (se for transferência entre armazéns)
        /// </summary>
        public int? OriginWarehouseId { get; set; }

        /// <summary>
        /// Indicador de urgência da carga
        /// </summary>
        public ASNPriority Priority { get; set; } = ASNPriority.Normal;

        /// <summary>
        /// Referência externa (pedido de compra, nota de transferência, etc)
        /// </summary>
        public string? ExternalReference { get; set; }

        /// <summary>
        /// Indicador de que a carga foi inspecionada
        /// </summary>
        public bool IsInspected { get; set; }

        /// <summary>
        /// Data da inspeção
        /// </summary>
        public DateTime? InspectionDate { get; set; }

        /// <summary>
        /// ID do usuário responsável pela inspeção
        /// </summary>
        public int? InspectedByUserId { get; set; }

        /// <summary>
        /// Resultado da inspeção
        /// </summary>
        public InspectionResult? InspectionResult { get; set; }

        /// <summary>
        /// Observações da inspeção
        /// </summary>
        public string? InspectionNotes { get; set; }

        /// <summary>
        /// Coleção de itens desta ASN
        /// </summary>
        public List<ASNItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Enumeração dos status possíveis de uma ASN
    /// </summary>
    public enum ASNStatus
    {
        /// <summary>
        /// ASN agendada - Carga ainda não chegou
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// Carga em trânsito
        /// </summary>
        InTransit = 2,

        /// <summary>
        /// Carga chegou, aguardando descarregamento
        /// </summary>
        Arrived = 3,

        /// <summary>
        /// Carga sendo descarregada
        /// </summary>
        Unloading = 4,

        /// <summary>
        /// Carga recebida e conferida
        /// </summary>
        Received = 5,

        /// <summary>
        /// Carga parcialmente recebida (parte rejeitada)
        /// </summary>
        PartiallyReceived = 6,

        /// <summary>
        /// Carga rejeitada
        /// </summary>
        Rejected = 7,

        /// <summary>
        /// ASN cancelada
        /// </summary>
        Cancelled = 8
    }

    /// <summary>
    /// Enumeração de prioridades de ASN
    /// </summary>
    public enum ASNPriority
    {
        /// <summary>
        /// Prioridade baixa
        /// </summary>
        Low = 1,

        /// <summary>
        /// Prioridade normal
        /// </summary>
        Normal = 2,

        /// <summary>
        /// Prioridade alta
        /// </summary>
        High = 3,

        /// <summary>
        /// Prioridade crítica
        /// </summary>
        Critical = 4
    }

    /// <summary>
    /// Enumeração de resultado de inspeção
    /// </summary>
    public enum InspectionResult
    {
        /// <summary>
        /// Aprovado sem ressalvas
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Aprovado com ressalvas
        /// </summary>
        ApprovedWithRemarks = 2,

        /// <summary>
        /// Rejeitado - problemas encontrados
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Inspecionado parcialmente
        /// </summary>
        PartialInspection = 4
    }
}

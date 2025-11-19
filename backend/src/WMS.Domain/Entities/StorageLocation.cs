using WMS.Domain.Enums;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa uma localização física de armazenamento no armazém
    /// Cada localização é um espaço discreto onde produtos podem ser armazenados
    /// Exemplo: A-001-01-01 (Corredor A, Estante 001, Prateleira 01, Posição 01)
    /// </summary>
    public class StorageLocation : BaseEntity
    {
        /// <summary>
        /// ID do armazém ao qual esta localização pertence
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Código único da localização
        /// Formato: [Corredor]-[Estante]-[Prateleira]-[Posição]
        /// Exemplo: A-001-01-01
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Zona de armazenamento à qual esta localização pertence
        /// (Picking, Reserva, Cross-Dock, Quarentena, etc)
        /// </summary>
        public StorageZone Zone { get; set; }

        /// <summary>
        /// Status atual da localização
        /// (Disponível, Ocupada, Indisponível)
        /// </summary>
        public LocationStatus Status { get; set; }

        /// <summary>
        /// ID do produto atualmente armazenado nesta localização
        /// Null se a localização está vazia
        /// </summary>
        public int? CurrentProductId { get; set; }

        /// <summary>
        /// Quantidade atual de unidades armazenadas nesta localização
        /// </summary>
        public int CurrentQuantity { get; set; }

        /// <summary>
        /// Capacidade máxima em kg desta localização
        /// </summary>
        public decimal MaxCapacityKg { get; set; }

        /// <summary>
        /// Capacidade máxima em unidades desta localização
        /// </summary>
        public int MaxCapacityUnits { get; set; }

        /// <summary>
        /// Posição da linha/corredor (A, B, C, etc - convertido para índice numérico)
        /// </summary>
        public int RowPosition { get; set; }

        /// <summary>
        /// Posição da coluna/estante (número)
        /// </summary>
        public int ColumnPosition { get; set; }

        /// <summary>
        /// Posição do nível/prateleira (número)
        /// 0 = Chão, 1 = 1ª prateleira, etc
        /// </summary>
        public int LevelPosition { get; set; }

        /// <summary>
        /// Número do corredor para fácil navegação
        /// </summary>
        public string? CorridorNumber { get; set; }

        /// <summary>
        /// Número da estante/rack
        /// </summary>
        public string? RackNumber { get; set; }

        /// <summary>
        /// Indica se esta localização está bloqueada para uso
        /// Localização bloqueada não pode receber novos produtos
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Motivo do bloqueio (se aplicável)
        /// Exemplo: "Em manutenção", "Danificada", "Reservada para teste"
        /// </summary>
        public string? BlockReason { get; set; }

        /// <summary>
        /// Última data em que um produto foi retirado desta localização
        /// Utilizado para otimização de picking
        /// </summary>
        public DateTime? LastRemovalDate { get; set; }

        /// <summary>
        /// Última data em que um produto foi adicionado a esta localização
        /// </summary>
        public DateTime? LastAdditionDate { get; set; }

        /// <summary>
        /// Número de vezes que esta localização foi acessada
        /// Métrica para otimização
        /// </summary>
        public int AccessCount { get; set; }

        /// <summary>
        /// Altitude/profundidade da localização em relação ao ponto de consolidação
        /// Utilizado para cálculo de distância de picking
        /// </summary>
        public decimal? DistanceFromConsolidationPoint { get; set; }

        /// <summary>
        /// Indica se a localização requer temperatura controlada
        /// </summary>
        public bool RequiresTemperatureControl { get; set; }

        /// <summary>
        /// Temperatura ideal para esta localização (em Celsius)
        /// </summary>
        public decimal? IdealTemperature { get; set; }

        /// <summary>
        /// Umidade relativa ideal para esta localização (em percentual)
        /// </summary>
        public decimal? IdealHumidity { get; set; }
    }

    /// <summary>
    /// Enumeração do status de uma localização de armazenamento
    /// </summary>
    public enum LocationStatus
    {
        /// <summary>
        /// Localização disponível para receber produtos
        /// </summary>
        Available = 1,

        /// <summary>
        /// Localização ocupada com produto(s)
        /// </summary>
        Occupied = 2,

        /// <summary>
        /// Localização indisponível (danificada, em manutenção, etc)
        /// </summary>
        Unavailable = 3,

        /// <summary>
        /// Localização parcialmente ocupada
        /// Pode receber mais produtos até atingir capacidade máxima
        /// </summary>
        PartiallyOccupied = 4
    }
}

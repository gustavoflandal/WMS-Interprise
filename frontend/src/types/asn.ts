// ============================================================================
// Tipos TypeScript para ASN (Advanced Shipping Notice)
// Baseados nas entidades do backend
// ============================================================================

export enum ASNStatus {
  Scheduled = 1,
  InTransit = 2,
  Arrived = 3,
  Unloading = 4,
  Received = 5,
  PartiallyReceived = 6,
  Rejected = 7,
  Cancelled = 8,
}

export enum ASNPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Critical = 4,
}

export enum InspectionResult {
  Approved = 1,
  ApprovedWithRemarks = 2,
  Rejected = 3,
  PartialInspection = 4,
}

export enum ItemQualityStatus {
  Pending = 1,
  Accepted = 2,
  AcceptedWithRemarks = 3,
  Rejected = 4,
  ReturnedToSupplier = 5,
  InQuarantine = 6,
}

// Labels em português para os enums
export const ASNStatusLabels: Record<ASNStatus, string> = {
  [ASNStatus.Scheduled]: 'Agendada',
  [ASNStatus.InTransit]: 'Em Trânsito',
  [ASNStatus.Arrived]: 'Chegou',
  [ASNStatus.Unloading]: 'Descarregando',
  [ASNStatus.Received]: 'Recebida',
  [ASNStatus.PartiallyReceived]: 'Parcialmente Recebida',
  [ASNStatus.Rejected]: 'Rejeitada',
  [ASNStatus.Cancelled]: 'Cancelada',
};

export const ASNPriorityLabels: Record<ASNPriority, string> = {
  [ASNPriority.Low]: 'Baixa',
  [ASNPriority.Normal]: 'Normal',
  [ASNPriority.High]: 'Alta',
  [ASNPriority.Critical]: 'Crítica',
};

export const InspectionResultLabels: Record<InspectionResult, string> = {
  [InspectionResult.Approved]: 'Aprovado',
  [InspectionResult.ApprovedWithRemarks]: 'Aprovado com Ressalvas',
  [InspectionResult.Rejected]: 'Rejeitado',
  [InspectionResult.PartialInspection]: 'Inspeção Parcial',
};

export const ItemQualityStatusLabels: Record<ItemQualityStatus, string> = {
  [ItemQualityStatus.Pending]: 'Pendente',
  [ItemQualityStatus.Accepted]: 'Aceito',
  [ItemQualityStatus.AcceptedWithRemarks]: 'Aceito com Ressalvas',
  [ItemQualityStatus.Rejected]: 'Rejeitado',
  [ItemQualityStatus.ReturnedToSupplier]: 'Devolvido',
  [ItemQualityStatus.InQuarantine]: 'Em Quarentena',
};

// Cores para badges de status
export const ASNStatusColors: Record<ASNStatus, 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success'> = {
  [ASNStatus.Scheduled]: 'default',
  [ASNStatus.InTransit]: 'info',
  [ASNStatus.Arrived]: 'primary',
  [ASNStatus.Unloading]: 'warning',
  [ASNStatus.Received]: 'success',
  [ASNStatus.PartiallyReceived]: 'warning',
  [ASNStatus.Rejected]: 'error',
  [ASNStatus.Cancelled]: 'default',
};

export const ASNPriorityColors: Record<ASNPriority, 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success'> = {
  [ASNPriority.Low]: 'default',
  [ASNPriority.Normal]: 'info',
  [ASNPriority.High]: 'warning',
  [ASNPriority.Critical]: 'error',
};

// Interface para Item de ASN
export interface ASNItem {
  id: string;
  asnId: string;
  productId: number;
  expectedQuantity: number;
  receivedQuantity: number;
  unit: string;
  expectedWeight?: number;
  expectedVolume?: number;
  expiryDate?: string;
  lotNumber?: string;
  serialNumber?: string;
  isConformed: boolean;
  qualityStatus: ItemQualityStatus;
  rejectionReason?: string;
  notes?: string;
  unitValue?: number;
  requiresQualityInspection: boolean;
  receivedByUserId?: number;
  receivedAt?: string;
  quantityVariance: number;
  variancePercentage: number;
  createdAt: string;
  updatedAt: string;
}

// Interface principal para ASN
export interface ASN {
  id: string;
  warehouseId: number;
  asnNumber: string;
  supplierId?: number;
  invoiceNumber?: string;
  scheduledArrivalDate: string;
  actualArrivalDate?: string;
  status: ASNStatus;
  expectedItemCount: number;
  expectedTotalWeight: number;
  expectedTotalVolume?: number;
  receivedItemCount: number;
  notes?: string;
  createdByUserId?: number;
  originWarehouseId?: number;
  priority: ASNPriority;
  externalReference?: string;
  isInspected: boolean;
  inspectionDate?: string;
  inspectedByUserId?: number;
  inspectionResult?: InspectionResult;
  inspectionNotes?: string;
  items: ASNItem[];
  createdAt: string;
  updatedAt: string;
}

// DTOs para criação
export interface CreateASNItemRequest {
  productId: number;
  expectedQuantity: number;
  unit?: string;
  expectedWeight?: number;
  expectedVolume?: number;
  expiryDate?: string;
  lotNumber?: string;
  serialNumber?: string;
  unitValue?: number;
  requiresQualityInspection?: boolean;
  notes?: string;
}

export interface CreateASNRequest {
  warehouseId: number;
  supplierId?: number;
  invoiceNumber?: string;
  scheduledArrivalDate: string;
  priority?: ASNPriority;
  externalReference?: string;
  originWarehouseId?: number;
  notes?: string;
  items: CreateASNItemRequest[];
}

// DTOs para atualização
export interface UpdateASNRequest {
  supplierId?: number;
  invoiceNumber?: string;
  scheduledArrivalDate?: string;
  status?: ASNStatus;
  priority?: ASNPriority;
  externalReference?: string;
  notes?: string;
}

// DTO para atualizar item
export interface UpdateASNItemRequest {
  expectedQuantity?: number;
  receivedQuantity?: number;
  unit?: string;
  expectedWeight?: number;
  expectedVolume?: number;
  expiryDate?: string;
  lotNumber?: string;
  serialNumber?: string;
  qualityStatus?: ItemQualityStatus;
  rejectionReason?: string;
  notes?: string;
  unitValue?: number;
}

// DTO para registrar inspeção
export interface RegisterInspectionRequest {
  inspectionResult: InspectionResult;
  inspectionNotes?: string;
}

// DTO para confirmar chegada
export interface ConfirmArrivalRequest {
  actualArrivalDate: string;
  notes?: string;
}

// ============================================================================
// Tipos TypeScript para Receipt (Recebimento Físico)
// Baseados nas entidades do backend
// ============================================================================

export enum ReceiptStatus {
  Draft = 1,
  InProgress = 2,
  Confirmed = 3,
  Closed = 4,
  Cancelled = 5,
  OnHold = 6,
}

export enum ReceiptType {
  Purchase = 1,
  TransferBetweenWarehouses = 2,
  CustomerReturn = 3,
  SupplierReturn = 4,
  InternalReplenishment = 5,
  Adjustment = 6,
}

export enum ReceiptItemQualityStatus {
  Accepted = 1,
  AcceptedWithRemarks = 2,
  Rejected = 3,
  PartiallyAccepted = 4,
  ReturnedToSupplier = 5,
  UnderInspection = 6,
  PendingInspection = 7,
}

// Labels em português para os enums
export const ReceiptStatusLabels: Record<ReceiptStatus, string> = {
  [ReceiptStatus.Draft]: 'Rascunho',
  [ReceiptStatus.InProgress]: 'Em Andamento',
  [ReceiptStatus.Confirmed]: 'Confirmado',
  [ReceiptStatus.Closed]: 'Fechado',
  [ReceiptStatus.Cancelled]: 'Cancelado',
  [ReceiptStatus.OnHold]: 'Em Espera',
};

export const ReceiptTypeLabels: Record<ReceiptType, string> = {
  [ReceiptType.Purchase]: 'Compra',
  [ReceiptType.TransferBetweenWarehouses]: 'Transferência',
  [ReceiptType.CustomerReturn]: 'Devolução Cliente',
  [ReceiptType.SupplierReturn]: 'Devolução Fornecedor',
  [ReceiptType.InternalReplenishment]: 'Reabastecimento',
  [ReceiptType.Adjustment]: 'Ajuste',
};

export const ReceiptItemQualityStatusLabels: Record<ReceiptItemQualityStatus, string> = {
  [ReceiptItemQualityStatus.Accepted]: 'Aceito',
  [ReceiptItemQualityStatus.AcceptedWithRemarks]: 'Aceito com Ressalvas',
  [ReceiptItemQualityStatus.Rejected]: 'Rejeitado',
  [ReceiptItemQualityStatus.PartiallyAccepted]: 'Parcialmente Aceito',
  [ReceiptItemQualityStatus.ReturnedToSupplier]: 'Devolvido',
  [ReceiptItemQualityStatus.UnderInspection]: 'Em Inspeção',
  [ReceiptItemQualityStatus.PendingInspection]: 'Pendente Inspeção',
};

// Cores para badges de status
export const ReceiptStatusColors: Record<ReceiptStatus, 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success'> = {
  [ReceiptStatus.Draft]: 'default',
  [ReceiptStatus.InProgress]: 'warning',
  [ReceiptStatus.Confirmed]: 'info',
  [ReceiptStatus.Closed]: 'success',
  [ReceiptStatus.Cancelled]: 'error',
  [ReceiptStatus.OnHold]: 'warning',
};

export const ReceiptTypeColors: Record<ReceiptType, 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success'> = {
  [ReceiptType.Purchase]: 'primary',
  [ReceiptType.TransferBetweenWarehouses]: 'info',
  [ReceiptType.CustomerReturn]: 'warning',
  [ReceiptType.SupplierReturn]: 'error',
  [ReceiptType.InternalReplenishment]: 'secondary',
  [ReceiptType.Adjustment]: 'default',
};

export const ReceiptItemQualityStatusColors: Record<ReceiptItemQualityStatus, 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success'> = {
  [ReceiptItemQualityStatus.Accepted]: 'success',
  [ReceiptItemQualityStatus.AcceptedWithRemarks]: 'info',
  [ReceiptItemQualityStatus.Rejected]: 'error',
  [ReceiptItemQualityStatus.PartiallyAccepted]: 'warning',
  [ReceiptItemQualityStatus.ReturnedToSupplier]: 'error',
  [ReceiptItemQualityStatus.UnderInspection]: 'warning',
  [ReceiptItemQualityStatus.PendingInspection]: 'default',
};

// Interface para Item de Recebimento
export interface ReceiptItem {
  id: string;
  receiptDocumentationId: string;
  productId: number;
  quantityReceived: number;
  quantityExpected?: number;
  unit: string;
  actualWeight: number;
  actualVolume?: number;
  lotNumber?: string;
  expiryDate?: string;
  serialNumbers?: string;
  storageLocationId: number;
  qualityStatus: ReceiptItemQualityStatus;
  rejectionReason?: string;
  notes?: string;
  declaredUnitValue?: number;
  totalValue?: number;
  isInspected: boolean;
  inspectedByUserId?: number;
  inspectedAt?: string;
  temperatureAtReceipt?: number;
  humidityAtReceipt?: number;
  evidencePhotoUrl?: string;
  hasDiscrepancy: boolean;
  discrepancyQuantity: number;
  discrepancyPercentage: number;
  createdAt: string;
  updatedAt: string;
}

// Interface principal para Receipt
export interface Receipt {
  id: string;
  warehouseId: number;
  asnId?: number;
  receiptNumber: string;
  receiptDate: string;
  status: ReceiptStatus;
  totalItemsReceived: number;
  totalWeight: number;
  totalVolume?: number;
  operatorId: number;
  supervisorId?: number;
  invoiceNumber?: string;
  externalReference?: string;
  notes?: string;
  confirmedAt?: string;
  confirmedByUserId?: number;
  closedAt?: string;
  closedByUserId?: number;
  itemsWithDiscrepancy: number;
  rejectedItems: number;
  hasDiscrepancies: boolean;
  totalReceiptTimeMinutes?: number;
  receiptType: ReceiptType;
  supplierId?: number;
  carrier?: string;
  trackingNumber?: string;
  items: ReceiptItem[];
  createdAt: string;
  updatedAt: string;
}

// DTOs para criação
export interface CreateReceiptItemRequest {
  productId: number;
  quantityReceived: number;
  quantityExpected?: number;
  unit?: string;
  actualWeight: number;
  actualVolume?: number;
  lotNumber?: string;
  expiryDate?: string;
  serialNumbers?: string;
  storageLocationId: number;
  qualityStatus?: ReceiptItemQualityStatus;
  rejectionReason?: string;
  notes?: string;
  declaredUnitValue?: number;
  temperatureAtReceipt?: number;
  humidityAtReceipt?: number;
}

export interface CreateReceiptRequest {
  warehouseId: number;
  asnId?: number;
  receiptDate: string;
  receiptType: ReceiptType;
  operatorId: number;
  supervisorId?: number;
  invoiceNumber?: string;
  externalReference?: string;
  supplierId?: number;
  carrier?: string;
  trackingNumber?: string;
  notes?: string;
  items: CreateReceiptItemRequest[];
}

// DTOs para atualização
export interface UpdateReceiptRequest {
  status?: ReceiptStatus;
  supervisorId?: number;
  invoiceNumber?: string;
  externalReference?: string;
  carrier?: string;
  trackingNumber?: string;
  notes?: string;
}

// DTO para atualizar item
export interface UpdateReceiptItemRequest {
  quantityReceived?: number;
  actualWeight?: number;
  actualVolume?: number;
  lotNumber?: string;
  expiryDate?: string;
  serialNumbers?: string;
  storageLocationId?: number;
  qualityStatus?: ReceiptItemQualityStatus;
  rejectionReason?: string;
  notes?: string;
  declaredUnitValue?: number;
  temperatureAtReceipt?: number;
  humidityAtReceipt?: number;
}

// DTO para confirmar recebimento
export interface ConfirmReceiptRequest {
  supervisorId?: number;
  notes?: string;
}

// DTO para fechar recebimento
export interface CloseReceiptRequest {
  totalReceiptTimeMinutes?: number;
  notes?: string;
}

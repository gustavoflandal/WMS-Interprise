// ============================================================================
// Tipos e Interfaces para Produtos
// ============================================================================

export enum ProductCategory {
  Dry = 1,
  Refrigerated = 2,
  Frozen = 3,
  Perishable = 4,
  Controlled = 5,
  BulkVolume = 6,
  SmallVolume = 7,
  HighValue = 8,
}

export enum ProductType {
  Commodity = 1,
  Fractionable = 2,
  Fragile = 3,
  Heavy = 4,
  Liquid = 5,
  Gaseous = 6,
  Bulky = 7,
  IsolationRequired = 8,
}

export enum StorageZone {
  Picking = 1,
  Reserve = 2,
  CrossDock = 3,
  Quarantine = 4,
  Damage = 5,
  Refrigerated = 6,
  Frozen = 7,
  Controlled = 8,
  Consolidation = 9,
  Packing = 10,
}

export enum ABCClassification {
  A = 1,
  B = 2,
  C = 3,
}

export interface Product {
  id: string;
  sku: string;
  name: string;
  category: ProductCategory;
  type: ProductType;
  unitWeight: number;
  unitVolume: number;
  defaultStorageZone: StorageZone;
  requiresLotTracking: boolean;
  requiresSerialNumber: boolean;
  shelfLifeDays: number | null;
  minStorageTemperature: number | null;
  maxStorageTemperature: number | null;
  minStorageHumidity: number | null;
  maxStorageHumidity: number | null;
  isFlammable: boolean;
  isDangerous: boolean;
  isPharmaceutical: boolean;
  abcClassification: ABCClassification | null;
  unitCost: number | null;
  unitPrice: number | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  sku: string;
  name: string;
  category: number;
  type: number;
  unitWeight: number;
  unitVolume: number;
  defaultStorageZone: number;
  requiresLotTracking: boolean;
  requiresSerialNumber: boolean;
  shelfLifeDays?: number;
  minStorageTemperature?: number;
  maxStorageTemperature?: number;
  minStorageHumidity?: number;
  maxStorageHumidity?: number;
  isFlammable: boolean;
  isDangerous: boolean;
  isPharmaceutical: boolean;
  abcClassification?: number;
  unitCost?: number;
  unitPrice?: number;
}

export interface UpdateProductRequest {
  name?: string;
  category?: number;
  type?: number;
  unitWeight?: number;
  unitVolume?: number;
  defaultStorageZone?: number;
  requiresLotTracking?: boolean;
  requiresSerialNumber?: boolean;
  shelfLifeDays?: number;
  minStorageTemperature?: number;
  maxStorageTemperature?: number;
  minStorageHumidity?: number;
  maxStorageHumidity?: number;
  isFlammable?: boolean;
  isDangerous?: boolean;
  isPharmaceutical?: boolean;
  abcClassification?: number;
  unitCost?: number;
  unitPrice?: number;
  isActive?: boolean;
}

// Labels para exibição
export const ProductCategoryLabels: Record<ProductCategory, string> = {
  [ProductCategory.Dry]: 'Seco',
  [ProductCategory.Refrigerated]: 'Refrigerado',
  [ProductCategory.Frozen]: 'Congelado',
  [ProductCategory.Perishable]: 'Perecível',
  [ProductCategory.Controlled]: 'Controlado',
  [ProductCategory.BulkVolume]: 'Grande Volume',
  [ProductCategory.SmallVolume]: 'Pequeno Volume',
  [ProductCategory.HighValue]: 'Alto Valor',
};

export const ProductTypeLabels: Record<ProductType, string> = {
  [ProductType.Commodity]: 'Commodity',
  [ProductType.Fractionable]: 'Fracionável',
  [ProductType.Fragile]: 'Frágil',
  [ProductType.Heavy]: 'Pesado',
  [ProductType.Liquid]: 'Líquido',
  [ProductType.Gaseous]: 'Gasoso',
  [ProductType.Bulky]: 'Volumoso',
  [ProductType.IsolationRequired]: 'Isolamento Requerido',
};

export const StorageZoneLabels: Record<StorageZone, string> = {
  [StorageZone.Picking]: 'Picking',
  [StorageZone.Reserve]: 'Reserva',
  [StorageZone.CrossDock]: 'Cross-Dock',
  [StorageZone.Quarantine]: 'Quarentena',
  [StorageZone.Damage]: 'Danos/Devoluções',
  [StorageZone.Refrigerated]: 'Refrigerado',
  [StorageZone.Frozen]: 'Congelado',
  [StorageZone.Controlled]: 'Controlado',
  [StorageZone.Consolidation]: 'Consolidação',
  [StorageZone.Packing]: 'Packing',
};

export const ABCClassificationLabels: Record<ABCClassification, string> = {
  [ABCClassification.A]: 'A - Alto Giro',
  [ABCClassification.B]: 'B - Médio Giro',
  [ABCClassification.C]: 'C - Baixo Giro',
};

import httpClient from './httpClient';

// ============================================================================
// Types
// ============================================================================

export interface Warehouse {
  id: string;
  // Informações Básicas
  nome: string;
  codigo: string;
  descricao: string | null;
  
  // Localização
  endereco: string | null;
  cidade: string | null;
  estado: string | null;
  cep: string | null;
  pais: string;
  latitude: number | null;
  longitude: number | null;
  
  // Capacidade
  totalPosicoes: number | null;
  capacidadePesoTotal: number | null;
  
  // Operação
  horarioAbertura: string | null; // TimeSpan formato "HH:mm:ss"
  horarioFechamento: string | null; // TimeSpan formato "HH:mm:ss"
  maxTrabalhadores: number | null;
  
  // Status
  status: number; // 1=Ativo, 2=Inativo, 3=EmManutencao
  statusDescricao: string;
  
  // Auditoria
  criadoPor: string | null;
  atualizadoPor: string | null;
  
  // Metadata
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateWarehouseRequest {
  // Informações Básicas
  nome: string;
  codigo: string;
  descricao?: string;
  
  // Localização
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  pais?: string;
  latitude?: number;
  longitude?: number;
  
  // Capacidade
  totalPosicoes?: number;
  capacidadePesoTotal?: number;
  
  // Operação
  horarioAbertura?: string; // TimeSpan formato "HH:mm:ss"
  horarioFechamento?: string; // TimeSpan formato "HH:mm:ss"
  maxTrabalhadores?: number;
}

export interface UpdateWarehouseRequest {
  // Informações Básicas (código não pode ser alterado)
  nome: string;
  descricao?: string;
  
  // Localização
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  pais?: string;
  latitude?: number;
  longitude?: number;
  
  // Capacidade
  totalPosicoes?: number;
  capacidadePesoTotal?: number;
  
  // Operação
  horarioAbertura?: string; // TimeSpan formato "HH:mm:ss"
  horarioFechamento?: string; // TimeSpan formato "HH:mm:ss"
  maxTrabalhadores?: number;
  
  // Status
  status: number; // 1=Ativo, 2=Inativo, 3=EmManutencao
}

// ============================================================================
// Serviço de Armazém
// ============================================================================

export const warehouseService = {
  /**
   * Buscar todos os armazéns do tenant
   */
  getAll: async (): Promise<Warehouse[]> => {
    const response = await httpClient.get('/warehouse');
    return response.data;
  },

  /**
   * Buscar armazém por ID
   */
  getById: async (id: string): Promise<Warehouse> => {
    const response = await httpClient.get(`/warehouse/${id}`);
    return response.data;
  },

  /**
   * Criar novo armazém
   */
  create: async (request: CreateWarehouseRequest): Promise<Warehouse> => {
    const response = await httpClient.post('/warehouse', request);
    return response.data;
  },

  /**
   * Atualizar armazém existente
   */
  update: async (id: string, request: UpdateWarehouseRequest): Promise<Warehouse> => {
    const response = await httpClient.put(`/warehouse/${id}`, request);
    return response.data;
  },

  /**
   * Deletar armazém (soft delete)
   */
  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/warehouse/${id}`);
  },
};

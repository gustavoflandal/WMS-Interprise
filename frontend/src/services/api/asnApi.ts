import httpClient from './httpClient';
import {
  ASN,
  CreateASNRequest,
  UpdateASNRequest,
  RegisterInspectionRequest,
  ConfirmArrivalRequest,
  UpdateASNItemRequest,
} from '../../types/asn';

// ============================================================================
// Serviço de API para ASN (Advanced Shipping Notice)
// ============================================================================

class ASNService {
  private readonly basePath = '/ASN';

  /**
   * Busca todas as ASNs
   */
  async getAll(): Promise<ASN[]> {
    const response = await httpClient.get<ASN[]>(this.basePath);
    return response.data;
  }

  /**
   * Busca uma ASN por ID
   */
  async getById(id: string): Promise<ASN> {
    const response = await httpClient.get<ASN>(`${this.basePath}/${id}`);
    return response.data;
  }

  /**
   * Busca ASNs por armazém
   */
  async getByWarehouse(warehouseId: number): Promise<ASN[]> {
    const response = await httpClient.get<ASN[]>(`${this.basePath}/warehouse/${warehouseId}`);
    return response.data;
  }

  /**
   * Busca ASNs por status
   */
  async getByStatus(status: number): Promise<ASN[]> {
    const response = await httpClient.get<ASN[]>(`${this.basePath}/status/${status}`);
    return response.data;
  }

  /**
   * Busca ASNs por fornecedor
   */
  async getBySupplier(supplierId: number): Promise<ASN[]> {
    const response = await httpClient.get<ASN[]>(`${this.basePath}/supplier/${supplierId}`);
    return response.data;
  }

  /**
   * Busca ASNs agendadas para uma data específica
   */
  async getScheduledForDate(date: string): Promise<ASN[]> {
    const response = await httpClient.get<ASN[]>(`${this.basePath}/scheduled/${date}`);
    return response.data;
  }

  /**
   * Busca ASN por número
   */
  async getByAsnNumber(asnNumber: string): Promise<ASN> {
    const response = await httpClient.get<ASN>(`${this.basePath}/number/${asnNumber}`);
    return response.data;
  }

  /**
   * Verifica se um número de ASN já existe
   */
  async checkAsnNumberExists(asnNumber: string): Promise<boolean> {
    try {
      await this.getByAsnNumber(asnNumber);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Cria uma nova ASN
   */
  async create(data: CreateASNRequest): Promise<ASN> {
    const response = await httpClient.post<ASN>(this.basePath, data);
    return response.data;
  }

  /**
   * Atualiza uma ASN
   */
  async update(id: string, data: UpdateASNRequest): Promise<ASN> {
    const response = await httpClient.put<ASN>(`${this.basePath}/${id}`, data);
    return response.data;
  }

  /**
   * Deleta uma ASN
   */
  async delete(id: string): Promise<void> {
    await httpClient.delete(`${this.basePath}/${id}`);
  }

  /**
   * Confirma a chegada da ASN
   */
  async confirmArrival(id: string, data: ConfirmArrivalRequest): Promise<ASN> {
    const response = await httpClient.post<ASN>(`${this.basePath}/${id}/confirm-arrival`, data);
    return response.data;
  }

  /**
   * Inicia o descarregamento
   */
  async startUnloading(id: string): Promise<ASN> {
    const response = await httpClient.post<ASN>(`${this.basePath}/${id}/start-unloading`);
    return response.data;
  }

  /**
   * Registra inspeção
   */
  async registerInspection(id: string, data: RegisterInspectionRequest): Promise<ASN> {
    const response = await httpClient.post<ASN>(`${this.basePath}/${id}/register-inspection`, data);
    return response.data;
  }

  /**
   * Finaliza o recebimento
   */
  async completeReceiving(id: string): Promise<ASN> {
    const response = await httpClient.post<ASN>(`${this.basePath}/${id}/complete-receiving`);
    return response.data;
  }

  /**
   * Cancela uma ASN
   */
  async cancel(id: string, reason?: string): Promise<ASN> {
    const response = await httpClient.post<ASN>(`${this.basePath}/${id}/cancel`, { reason });
    return response.data;
  }

  /**
   * Atualiza um item da ASN
   */
  async updateItem(asnId: string, itemId: string, data: UpdateASNItemRequest): Promise<ASN> {
    const response = await httpClient.put<ASN>(`${this.basePath}/${asnId}/items/${itemId}`, data);
    return response.data;
  }

  /**
   * Remove um item da ASN
   */
  async deleteItem(asnId: string, itemId: string): Promise<ASN> {
    const response = await httpClient.delete<ASN>(`${this.basePath}/${asnId}/items/${itemId}`);
    return response.data;
  }
}

export const asnService = new ASNService();
export default asnService;

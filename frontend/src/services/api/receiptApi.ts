import httpClient from './httpClient';
import {
  Receipt,
  CreateReceiptRequest,
  UpdateReceiptRequest,
  ConfirmReceiptRequest,
  CloseReceiptRequest,
  UpdateReceiptItemRequest,
} from '../../types/receipt';

// ============================================================================
// Serviço de API para Receipt (Recebimento Físico)
// ============================================================================

class ReceiptService {
  private readonly basePath = '/Receipt';

  /**
   * Busca todos os recebimentos
   */
  async getAll(): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(this.basePath);
    return response.data;
  }

  /**
   * Busca um recebimento por ID
   */
  async getById(id: string): Promise<Receipt> {
    const response = await httpClient.get<Receipt>(`${this.basePath}/${id}`);
    return response.data;
  }

  /**
   * Busca recebimentos por armazém
   */
  async getByWarehouse(warehouseId: number): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(`${this.basePath}/warehouse/${warehouseId}`);
    return response.data;
  }

  /**
   * Busca recebimentos por status
   */
  async getByStatus(status: number): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(`${this.basePath}/status/${status}`);
    return response.data;
  }

  /**
   * Busca recebimentos por ASN
   */
  async getByAsn(asnId: number): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(`${this.basePath}/asn/${asnId}`);
    return response.data;
  }

  /**
   * Busca recebimentos por período
   */
  async getByDateRange(startDate: string, endDate: string): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(
      `${this.basePath}/date-range?startDate=${startDate}&endDate=${endDate}`
    );
    return response.data;
  }

  /**
   * Busca recebimentos por operador
   */
  async getByOperator(operatorId: number): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(`${this.basePath}/operator/${operatorId}`);
    return response.data;
  }

  /**
   * Busca recebimento por número
   */
  async getByReceiptNumber(receiptNumber: string): Promise<Receipt> {
    const response = await httpClient.get<Receipt>(`${this.basePath}/number/${receiptNumber}`);
    return response.data;
  }

  /**
   * Verifica se um número de recebimento já existe
   */
  async checkReceiptNumberExists(receiptNumber: string): Promise<boolean> {
    try {
      await this.getByReceiptNumber(receiptNumber);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Cria um novo recebimento
   */
  async create(data: CreateReceiptRequest): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(this.basePath, data);
    return response.data;
  }

  /**
   * Atualiza um recebimento
   */
  async update(id: string, data: UpdateReceiptRequest): Promise<Receipt> {
    const response = await httpClient.put<Receipt>(`${this.basePath}/${id}`, data);
    return response.data;
  }

  /**
   * Deleta um recebimento
   */
  async delete(id: string): Promise<void> {
    await httpClient.delete(`${this.basePath}/${id}`);
  }

  /**
   * Confirma o recebimento
   */
  async confirm(id: string, data: ConfirmReceiptRequest): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(`${this.basePath}/${id}/confirm`, data);
    return response.data;
  }

  /**
   * Fecha o recebimento
   */
  async close(id: string, data: CloseReceiptRequest): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(`${this.basePath}/${id}/close`, data);
    return response.data;
  }

  /**
   * Cancela o recebimento
   */
  async cancel(id: string, reason?: string): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(`${this.basePath}/${id}/cancel`, { reason });
    return response.data;
  }

  /**
   * Coloca o recebimento em espera
   */
  async putOnHold(id: string, reason?: string): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(`${this.basePath}/${id}/hold`, { reason });
    return response.data;
  }

  /**
   * Remove o recebimento da espera
   */
  async removeFromHold(id: string): Promise<Receipt> {
    const response = await httpClient.post<Receipt>(`${this.basePath}/${id}/resume`);
    return response.data;
  }

  /**
   * Atualiza um item do recebimento
   */
  async updateItem(receiptId: string, itemId: string, data: UpdateReceiptItemRequest): Promise<Receipt> {
    const response = await httpClient.put<Receipt>(`${this.basePath}/${receiptId}/items/${itemId}`, data);
    return response.data;
  }

  /**
   * Remove um item do recebimento
   */
  async deleteItem(receiptId: string, itemId: string): Promise<Receipt> {
    const response = await httpClient.delete<Receipt>(`${this.basePath}/${receiptId}/items/${itemId}`);
    return response.data;
  }

  /**
   * Busca recebimentos com discrepâncias
   */
  async getWithDiscrepancies(): Promise<Receipt[]> {
    const response = await httpClient.get<Receipt[]>(`${this.basePath}/discrepancies`);
    return response.data;
  }

  /**
   * Gera relatório de recebimento
   */
  async generateReport(id: string): Promise<Blob> {
    const response = await httpClient.get(`${this.basePath}/${id}/report`, {
      responseType: 'blob',
    });
    return response.data;
  }
}

export const receiptService = new ReceiptService();
export default receiptService;

import apiClient from './httpClient';
import { Product, CreateProductRequest, UpdateProductRequest } from '../../types/product';

// ============================================================================
// Serviço de API para Produtos
// ============================================================================

export const productService = {
  /**
   * Obtém todos os produtos
   */
  getAll: async (): Promise<Product[]> => {
    const response = await apiClient.get<Product[]>('/Products');
    return response.data;
  },

  /**
   * Obtém um produto por ID
   */
  getById: async (id: string): Promise<Product> => {
    const response = await apiClient.get<Product>(`/Products/${id}`);
    return response.data;
  },

  /**
   * Obtém um produto por SKU
   */
  getBySku: async (sku: string): Promise<Product> => {
    const response = await apiClient.get<Product>(`/Products/sku/${sku}`);
    return response.data;
  },

  /**
   * Cria um novo produto
   */
  create: async (data: CreateProductRequest): Promise<Product> => {
    const response = await apiClient.post<Product>('/Products', data);
    return response.data;
  },

  /**
   * Atualiza um produto existente
   */
  update: async (id: string, data: UpdateProductRequest): Promise<Product> => {
    const response = await apiClient.put<Product>(`/Products/${id}`, data);
    return response.data;
  },

  /**
   * Deleta um produto
   */
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/Products/${id}`);
  },

  /**
   * Verifica se um SKU já existe
   */
  checkSkuExists: async (sku: string): Promise<boolean> => {
    const response = await apiClient.get<boolean>(`/Products/check-sku/${sku}`);
    return response.data;
  },
};

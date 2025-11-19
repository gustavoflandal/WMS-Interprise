import httpClient from './httpClient';

// ============================================================================
// Types
// ============================================================================

export interface Company {
  id: string;
  // Dados Principais
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  inscricaoEstadual: string;
  inscricaoMunicipal: string;
  
  // Contato
  email: string;
  telefone: string;
  celular: string;
  website: string;
  
  // Endereço
  cep: string;
  logradouro: string;
  numero: string;
  complemento: string;
  bairro: string;
  cidade: string;
  estado: string;
  
  // Informações Adicionais
  dataAbertura: string;
  capitalSocial: string;
  atividadePrincipal: string;
  regimeTributario: string;
  
  // Responsável Legal
  nomeResponsavel: string;
  cpfResponsavel: string;
  emailResponsavel: string;
  telefoneResponsavel: string;
  cargoResponsavel: string;
  
  // Metadata
  createdAt: string;
  updatedAt: string;
}

export interface CreateCompanyRequest {
  // Dados Principais
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  inscricaoEstadual: string;
  inscricaoMunicipal: string;
  
  // Contato
  email: string;
  telefone: string;
  celular: string;
  website: string;
  
  // Endereço
  cep: string;
  logradouro: string;
  numero: string;
  complemento: string;
  bairro: string;
  cidade: string;
  estado: string;
  
  // Informações Adicionais
  dataAbertura: string;
  capitalSocial: string;
  atividadePrincipal: string;
  regimeTributario: string;
  
  // Responsável Legal
  nomeResponsavel: string;
  cpfResponsavel: string;
  emailResponsavel: string;
  telefoneResponsavel: string;
  cargoResponsavel: string;
}

export interface UpdateCompanyRequest {
  // Dados Principais (CNPJ e Razão Social não podem ser alterados)
  nomeFantasia?: string;
  inscricaoEstadual?: string;
  inscricaoMunicipal?: string;
  
  // Contato
  email?: string;
  telefone?: string;
  celular?: string;
  website?: string;
  
  // Endereço
  cep?: string;
  logradouro?: string;
  numero?: string;
  complemento?: string;
  bairro?: string;
  cidade?: string;
  estado?: string;
  
  // Informações Adicionais
  dataAbertura?: string;
  capitalSocial?: string;
  atividadePrincipal?: string;
  regimeTributario?: string;
  
  // Responsável Legal
  nomeResponsavel?: string;
  cpfResponsavel?: string;
  emailResponsavel?: string;
  telefoneResponsavel?: string;
  cargoResponsavel?: string;
}

// ============================================================================
// Serviço de Empresa
// ============================================================================

export const companyService = {
  /**
   * Buscar dados da empresa (sempre retorna apenas 1 empresa - do tenant)
   */
  get: async (): Promise<Company | null> => {
    try {
      const response = await httpClient.get('/company');
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  /**
   * Criar nova empresa
   */
  create: async (request: CreateCompanyRequest): Promise<Company> => {
    const response = await httpClient.post('/company', request);
    return response.data;
  },

  /**
   * Atualizar empresa existente
   */
  update: async (request: UpdateCompanyRequest): Promise<Company> => {
    const response = await httpClient.put('/company', request);
    return response.data;
  },

  /**
   * Deletar empresa (soft delete)
   */
  delete: async (): Promise<void> => {
    await httpClient.delete('/company');
  },
};

import apiClient from './httpClient';
import {
  LoginRequest,
  RegisterRequest,
  AuthenticationResponse,
  RefreshTokenRequest,
  ChangePasswordRequest,
} from '../../types/api';

// ============================================================================
// Serviço de Autenticação
// ============================================================================

export const authService = {
  login: async (request: LoginRequest): Promise<AuthenticationResponse> => {
    const response = await apiClient.post('/auth/login', request);
    return response.data;
  },

  register: async (request: RegisterRequest): Promise<AuthenticationResponse> => {
    const response = await apiClient.post('/auth/register', request);
    return response.data;
  },

  refreshToken: async (request: RefreshTokenRequest): Promise<AuthenticationResponse> => {
    const response = await apiClient.post('/auth/refresh-token', request);
    return response.data;
  },

  logout: async (): Promise<void> => {
    await apiClient.post('/auth/logout');
  },

  changePassword: async (request: ChangePasswordRequest): Promise<void> => {
    await apiClient.post('/auth/change-password', request);
  },
};

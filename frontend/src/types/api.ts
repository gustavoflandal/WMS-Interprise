// ============================================================================
// Types e Interfaces para a API
// ============================================================================

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface AuthenticationResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserResponse;
}

export interface UserResponse {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  lastLoginAt?: string;
  createdAt: string;
  roles: string[];
  permissions: string[];
}

export interface CreateUserRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  phone?: string;
  tenantId?: string;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  phone?: string;
  isActive?: boolean;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// ============================================================================
// Types para Customer
// ============================================================================

export interface Customer {
  id: string;
  nome: string;
  tipo: 'PJ' | 'PF';
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
  status: 'Ativo' | 'Inativo' | 'Bloqueado';
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCustomerRequest {
  nome: string;
  tipo: 'PJ' | 'PF';
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
}

export interface UpdateCustomerRequest {
  nome?: string;
  tipo?: 'PJ' | 'PF';
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
  status?: 'Ativo' | 'Inativo' | 'Bloqueado';
}

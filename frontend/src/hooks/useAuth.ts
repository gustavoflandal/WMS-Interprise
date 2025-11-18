import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { useAuthStore } from '../store/slices/authSlice';
import { authService } from '../services/api/authApi';
import { LoginRequest, RegisterRequest } from '../types/api';
import { toast } from 'react-toastify';

// ============================================================================
// Hook de Autenticação
// ============================================================================

export const useAuth = () => {
  const navigate = useNavigate();
  const {
    user,
    accessToken,
    refreshToken,
    isAuthenticated,
    setAuth,
    setTokens,
    setLoading,
    setError,
    logout: logoutStore,
    clearError,
  } = useAuthStore();

  // ========== Mutation de Login ==========
  const loginMutation = useMutation({
    mutationFn: async (request: LoginRequest) => {
      setLoading(true);
      try {
        const response = await authService.login(request);
        return response;
      } finally {
        setLoading(false);
      }
    },
    onSuccess: (data) => {
      setAuth(data.user, data.accessToken, data.refreshToken, data.expiresAt);
      toast.success('Login realizado com sucesso!');
      navigate('/dashboard');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao fazer login';
      setError(message);
      toast.error(message);
    },
  });

  // ========== Mutation de Registro ==========
  const registerMutation = useMutation({
    mutationFn: async (request: RegisterRequest) => {
      setLoading(true);
      try {
        const response = await authService.register(request);
        return response;
      } finally {
        setLoading(false);
      }
    },
    onSuccess: (data) => {
      setAuth(data.user, data.accessToken, data.refreshToken, data.expiresAt);
      toast.success('Registro realizado com sucesso!');
      navigate('/dashboard');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao registrar';
      setError(message);
      toast.error(message);
    },
  });

  // ========== Mutation de Logout ==========
  const logoutMutation = useMutation({
    mutationFn: async () => {
      try {
        await authService.logout();
      } catch (error) {
        // Fazer logout mesmo se a requisição falhar
        console.error('Erro ao fazer logout no servidor:', error);
      }
    },
    onSuccess: () => {
      logoutStore();
      toast.success('Logout realizado com sucesso');
      navigate('/login');
    },
  });

  // ========== Mutation de Refresh Token ==========
  // Não usado - refresh é feito automaticamente pelo interceptor
  // const refreshTokenMutation = useMutation({
  //   mutationFn: async () => {
  //     if (!refreshToken) {
  //       throw new Error('Refresh token não disponível');
  //     }
  //     const response = await authService.refreshToken({ refreshToken });
  //     return response;
  //   },
  //   onSuccess: (data) => {
  //     setTokens(data.accessToken, data.refreshToken, data.expiresIn);
  //   },
  //   onError: () => {
  //     logoutStore();
  //     navigate('/login');
  //   },
  // });

  // ========== Funções Públicas ==========
  const login = useCallback(
    (username: string, password: string) => {
      clearError();
      loginMutation.mutate({ username, password });
    },
    [loginMutation, clearError]
  );

  const register = useCallback(
    (data: RegisterRequest) => {
      clearError();
      registerMutation.mutate(data);
    },
    [registerMutation, clearError]
  );

  const logout = useCallback(() => {
    logoutMutation.mutate();
  }, [logoutMutation]);

  return {
    // State
    user,
    accessToken,
    refreshToken,
    isAuthenticated,
    isLoading: loginMutation.isPending || registerMutation.isPending || logoutMutation.isPending,
    error: loginMutation.error || registerMutation.error || logoutMutation.error,

    // Actions
    login,
    register,
    logout,
    clearError,
  };
};

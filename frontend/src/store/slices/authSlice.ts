import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { UserResponse } from '../../types/api';

// ============================================================================
// Estado de Autenticação
// ============================================================================

interface AuthState {
  user: UserResponse | null;
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Actions
  setAuth: (user: UserResponse, accessToken: string, refreshToken: string, expiresAt: string) => void;
  setTokens: (accessToken: string, refreshToken: string, expiresAt: string) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  logout: () => void;
  clearError: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      expiresAt: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,

      setAuth: (user: UserResponse, accessToken: string, refreshToken: string, expiresAt: string) =>
        set({
          user,
          accessToken,
          refreshToken,
          expiresAt,
          isAuthenticated: true,
          error: null,
        }),

      setTokens: (accessToken: string, refreshToken: string, expiresAt: string) => {
        set({
          accessToken,
          refreshToken,
          expiresAt,
          isAuthenticated: true,
          error: null,
        });
      },

      setLoading: (loading: boolean) => set({ isLoading: loading }),

      setError: (error: string | null) => set({ error }),

      clearError: () => set({ error: null }),

      logout: () =>
        set({
          user: null,
          accessToken: null,
          refreshToken: null,
          expiresAt: null,
          isAuthenticated: false,
          error: null,
        }),
    }),
    {
      name: 'auth-store',
      partialize: (state) => ({
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        expiresAt: state.expiresAt,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);

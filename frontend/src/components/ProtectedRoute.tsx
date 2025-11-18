import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../store/slices/authSlice';

// ============================================================================
// Componente de Rota Protegida
// ============================================================================

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, accessToken } = useAuthStore();
  
  // Se estiver autenticado, renderizar o componente
  if (isAuthenticated && accessToken) {
    return <>{children}</>;
  }

  // Se não estiver autenticado, redirecionar para login
  return <Navigate to="/login" replace />;
};

// ============================================================================
// Componente de Rota de Login (redireciona se já autenticado)
// ============================================================================

interface AuthRouteProps {
  children: React.ReactNode;
}

export const AuthRoute: React.FC<AuthRouteProps> = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

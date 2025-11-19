import { ThemeProvider, createTheme, CssBaseline } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

// Pages
import { LoginPage } from './pages/LoginPage';
import { Dashboard } from './pages/DashboardPage';
import { UsersPage } from './pages/UsersPage';
import { ActivitiesPage } from './pages/ActivitiesPage';
import { PermissionsPage } from './pages/PermissionsPage';
import { CompanyPage } from './pages/CompanyPage';
import { WarehousePage } from './pages/WarehousePage';
import { CustomerPage } from './pages/CustomerPage';
import { MainLayout } from './components/layout/MainLayout';
import { ProtectedRoute, AuthRoute } from './components/ProtectedRoute';

// ============================================================================
// Configuração de Tema
// ============================================================================

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
    background: {
      default: '#fafafa',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    h1: {
      fontWeight: 600,
    },
    h2: {
      fontWeight: 600,
    },
    h3: {
      fontWeight: 600,
    },
    h4: {
      fontWeight: 600,
    },
    h5: {
      fontWeight: 600,
    },
    h6: {
      fontWeight: 600,
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontWeight: 600,
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          boxShadow: '0 1px 3px rgba(0,0,0,0.08)',
        },
      },
    },
  },
});

// ============================================================================
// Configuração de React Query
// ============================================================================

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

// ============================================================================
// App Principal
// ============================================================================

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <BrowserRouter>
          <Routes>
            {/* Rotas de Autenticação */}
            <Route
              path="/login"
              element={
                <AuthRoute>
                  <LoginPage />
                </AuthRoute>
              }
            />

            {/* Rotas Protegidas */}
            <Route
              element={
                <ProtectedRoute>
                  <MainLayout />
                </ProtectedRoute>
              }
            >
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/customers" element={<CustomerPage />} />
              <Route path="/users/register" element={<UsersPage />} />
              <Route path="/users/activities" element={<ActivitiesPage />} />
              <Route path="/users/permissions" element={<PermissionsPage />} />
              <Route path="/company" element={<CompanyPage />} />
              <Route path="/warehouses" element={<WarehousePage />} />
            </Route>

            {/* Redirect padrão */}
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
          </Routes>

          {/* Toast Notifications */}
          <ToastContainer
            position="top-right"
            autoClose={3000}
            hideProgressBar={false}
            newestOnTop={true}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
          />
        </BrowserRouter>
      </ThemeProvider>
    </QueryClientProvider>
  );
}

export default App;

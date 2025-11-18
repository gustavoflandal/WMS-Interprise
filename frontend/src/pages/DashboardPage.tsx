import React from 'react';
import {
  Box,
  Container,
  Grid,
  Paper,
  Typography,
  Button,
} from '@mui/material';
import {
  TrendingUp,
  People,
  Inventory,
  LocalShipping,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

// ============================================================================
// Dashboard - Tela Principal
// ============================================================================

interface StatCard {
  title: string;
  value: number | string;
  icon: React.ReactNode;
  color: string;
  trend?: number;
}

export const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  // Mock data
  const stats: StatCard[] = [
    {
      title: 'Total de Usuários',
      value: 156,
      icon: <People sx={{ fontSize: 32 }} />,
      color: '#1976d2',
      trend: 12,
    },
    {
      title: 'Produtos em Estoque',
      value: 2841,
      icon: <Inventory sx={{ fontSize: 32 }} />,
      color: '#388e3c',
      trend: -5,
    },
    {
      title: 'Pedidos Processados',
      value: 1203,
      icon: <LocalShipping sx={{ fontSize: 32 }} />,
      color: '#f57c00',
      trend: 8,
    },
    {
      title: 'Taxa de Crescimento',
      value: '24.5%',
      icon: <TrendingUp sx={{ fontSize: 32 }} />,
      color: '#7b1fa2',
      trend: 3,
    },
  ];

  return (
    <Container maxWidth="lg">
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 1 }}>
          Bem-vindo, {user?.firstName}! 👋
        </Typography>
        <Typography variant="body1" color="textSecondary">
          Aqui está um resumo do seu sistema de gerenciamento de armazém.
        </Typography>
      </Box>

      {/* Stats Grid */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {stats.map((stat, index) => (
          <Grid item xs={12} sm={6} md={3} key={index}>
            <Paper
              sx={{
                p: 3,
                display: 'flex',
                alignItems: 'center',
                gap: 2,
                backgroundColor: '#f9f9f9',
                border: `2px solid ${stat.color}20`,
                borderRadius: 2,
                transition: 'transform 0.2s, box-shadow 0.2s',
                '&:hover': {
                  transform: 'translateY(-4px)',
                  boxShadow: '0 8px 16px rgba(0,0,0,0.1)',
                },
              }}
            >
              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  width: 60,
                  height: 60,
                  borderRadius: '50%',
                  backgroundColor: `${stat.color}15`,
                  color: stat.color,
                }}
              >
                {stat.icon}
              </Box>
              <Box>
                <Typography variant="body2" color="textSecondary" sx={{ mb: 0.5 }}>
                  {stat.title}
                </Typography>
                <Typography variant="h6" sx={{ fontWeight: 'bold', color: stat.color, mb: 0.5 }}>
                  {stat.value}
                </Typography>
                {stat.trend !== undefined && (
                  <Typography
                    variant="caption"
                    sx={{
                      color: stat.trend >= 0 ? '#388e3c' : '#d32f2f',
                      fontWeight: 'bold',
                    }}
                  >
                    {stat.trend >= 0 ? '↑' : '↓'} {Math.abs(stat.trend)}% este mês
                  </Typography>
                )}
              </Box>
            </Paper>
          </Grid>
        ))}
      </Grid>

      {/* Main Content Grid */}
      <Grid container spacing={3}>
        {/* Chart Placeholder */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2, fontWeight: 'bold' }}>
              📊 Desempenho Mensal
            </Typography>
            <Box
              sx={{
                height: 300,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: '#f5f5f5',
                borderRadius: 1,
                color: 'textSecondary',
              }}
            >
              <Typography>Gráfico de desempenho (Em desenvolvimento)</Typography>
            </Box>
          </Paper>
        </Grid>

        {/* Quick Actions */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2, fontWeight: 'bold' }}>
              ⚡ Ações Rápidas
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Button
                fullWidth
                variant="outlined"
                color="primary"
                onClick={() => navigate('/users/register')}
              >
                Gerenciar Usuários
              </Button>
              <Button
                fullWidth
                variant="outlined"
                color="primary"
              >
                Novo Pedido
              </Button>
              <Button
                fullWidth
                variant="outlined"
                color="primary"
              >
                Inventário
              </Button>
              <Button
                fullWidth
                variant="outlined"
                color="primary"
              >
                Relatórios
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

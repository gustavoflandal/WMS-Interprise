import React, { useState } from 'react';
import {
  Container,
  Paper,
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  TextField,
  InputAdornment,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  Grid,
} from '@mui/material';
import { Search as SearchIcon } from '@mui/icons-material';

// ============================================================================
// Página de Atividades
// ============================================================================

interface Activity {
  id: string;
  description: string;
  user: string;
  timestamp: string;
  type: 'create' | 'update' | 'delete' | 'login' | 'export' | 'import';
  module: string;
}

export const ActivitiesPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState<string>('all');
  const [filterModule, setFilterModule] = useState<string>('all');

  // Mock data - expandido para ter mais registros
  const allActivities: Activity[] = [
    {
      id: '1',
      description: 'Novo usuário criado',
      user: 'Maria Silva',
      timestamp: '2025-11-17 10:30',
      type: 'create',
      module: 'Usuários',
    },
    {
      id: '2',
      description: 'Perfil atualizado',
      user: 'João Santos',
      timestamp: '2025-11-17 09:15',
      type: 'update',
      module: 'Usuários',
    },
    {
      id: '3',
      description: 'Usuário removido',
      user: 'Sistema',
      timestamp: '2025-11-17 08:45',
      type: 'delete',
      module: 'Usuários',
    },
    {
      id: '4',
      description: 'Acesso realizado',
      user: 'Pedro Costa',
      timestamp: '2025-11-17 07:20',
      type: 'login',
      module: 'Sistema',
    },
    {
      id: '5',
      description: 'Dados exportados',
      user: 'Ana Oliveira',
      timestamp: '2025-11-16 16:10',
      type: 'export',
      module: 'Relatórios',
    },
    {
      id: '6',
      description: 'Produto adicionado ao estoque',
      user: 'Carlos Mendes',
      timestamp: '2025-11-16 15:45',
      type: 'create',
      module: 'Estoque',
    },
    {
      id: '7',
      description: 'Pedido atualizado',
      user: 'Fernanda Lima',
      timestamp: '2025-11-16 14:30',
      type: 'update',
      module: 'Pedidos',
    },
    {
      id: '8',
      description: 'Relatório exportado',
      user: 'Ricardo Alves',
      timestamp: '2025-11-16 13:20',
      type: 'export',
      module: 'Relatórios',
    },
    {
      id: '9',
      description: 'Dados importados',
      user: 'Juliana Rocha',
      timestamp: '2025-11-16 12:00',
      type: 'import',
      module: 'Sistema',
    },
    {
      id: '10',
      description: 'Configuração alterada',
      user: 'Admin',
      timestamp: '2025-11-16 11:15',
      type: 'update',
      module: 'Configurações',
    },
    {
      id: '11',
      description: 'Novo pedido criado',
      user: 'Marcos Paulo',
      timestamp: '2025-11-16 10:00',
      type: 'create',
      module: 'Pedidos',
    },
    {
      id: '12',
      description: 'Produto removido do estoque',
      user: 'Sistema',
      timestamp: '2025-11-16 09:30',
      type: 'delete',
      module: 'Estoque',
    },
    {
      id: '13',
      description: 'Acesso realizado',
      user: 'Laura Fernandes',
      timestamp: '2025-11-16 08:45',
      type: 'login',
      module: 'Sistema',
    },
    {
      id: '14',
      description: 'Relatório gerado',
      user: 'Bruno Cardoso',
      timestamp: '2025-11-15 17:20',
      type: 'create',
      module: 'Relatórios',
    },
    {
      id: '15',
      description: 'Estoque atualizado',
      user: 'Patrícia Souza',
      timestamp: '2025-11-15 16:00',
      type: 'update',
      module: 'Estoque',
    },
  ];

  const getActivityColor = (type: Activity['type']): 'info' | 'success' | 'error' | 'warning' | 'default' => {
    switch (type) {
      case 'create':
        return 'success';
      case 'update':
        return 'info';
      case 'delete':
        return 'error';
      case 'login':
        return 'warning';
      case 'export':
      case 'import':
        return 'default';
      default:
        return 'info';
    }
  };

  const getActivityLabel = (type: Activity['type']): string => {
    switch (type) {
      case 'create':
        return 'Criação';
      case 'update':
        return 'Atualização';
      case 'delete':
        return 'Remoção';
      case 'login':
        return 'Acesso';
      case 'export':
        return 'Exportação';
      case 'import':
        return 'Importação';
      default:
        return type;
    }
  };

  // Filtrar atividades
  const filteredActivities = allActivities.filter((activity) => {
    const matchesSearch = 
      activity.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      activity.user.toLowerCase().includes(searchTerm.toLowerCase()) ||
      activity.module.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesType = filterType === 'all' || activity.type === filterType;
    const matchesModule = filterModule === 'all' || activity.module === filterModule;

    return matchesSearch && matchesType && matchesModule;
  });

  // Obter módulos únicos para o filtro
  const uniqueModules = Array.from(new Set(allActivities.map(a => a.module))).sort();

  return (
    <Container maxWidth="lg">
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 1 }}>
          📝 Atividades do Sistema
        </Typography>
        <Typography variant="body1" color="textSecondary">
          Histórico completo de todas as ações realizadas no sistema.
        </Typography>
      </Box>

      {/* Filtros */}
      <Paper sx={{ mb: 3, p: 3 }}>
        <Grid container spacing={2}>
          {/* Busca */}
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              placeholder="Buscar por descrição, usuário ou módulo..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          {/* Filtro por Tipo */}
          <Grid item xs={12} sm={6} md={3}>
            <FormControl fullWidth>
              <InputLabel>Tipo de Atividade</InputLabel>
              <Select
                value={filterType}
                label="Tipo de Atividade"
                onChange={(e) => setFilterType(e.target.value)}
              >
                <MenuItem value="all">Todos</MenuItem>
                <MenuItem value="create">Criação</MenuItem>
                <MenuItem value="update">Atualização</MenuItem>
                <MenuItem value="delete">Remoção</MenuItem>
                <MenuItem value="login">Acesso</MenuItem>
                <MenuItem value="export">Exportação</MenuItem>
                <MenuItem value="import">Importação</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          {/* Filtro por Módulo */}
          <Grid item xs={12} sm={6} md={3}>
            <FormControl fullWidth>
              <InputLabel>Módulo</InputLabel>
              <Select
                value={filterModule}
                label="Módulo"
                onChange={(e) => setFilterModule(e.target.value)}
              >
                <MenuItem value="all">Todos</MenuItem>
                {uniqueModules.map((module) => (
                  <MenuItem key={module} value={module}>
                    {module}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>

        {/* Contador de resultados */}
        <Box sx={{ mt: 2 }}>
          <Typography variant="body2" color="textSecondary">
            Exibindo <strong>{filteredActivities.length}</strong> de <strong>{allActivities.length}</strong> atividades
          </Typography>
        </Box>
      </Paper>

      {/* Tabela de Atividades */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
              <TableCell sx={{ fontWeight: 'bold' }}>Data/Hora</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Descrição</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Usuário</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Módulo</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Tipo</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredActivities.length > 0 ? (
              filteredActivities.map((activity) => (
                <TableRow key={activity.id} hover>
                  <TableCell>
                    <Typography variant="body2" sx={{ whiteSpace: 'nowrap' }}>
                      {activity.timestamp}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">{activity.description}</Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 500 }}>
                      {activity.user}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={activity.module}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getActivityLabel(activity.type)}
                      size="small"
                      color={getActivityColor(activity.type)}
                    />
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={5} sx={{ textAlign: 'center', py: 4 }}>
                  <Typography color="textSecondary">
                    {searchTerm || filterType !== 'all' || filterModule !== 'all'
                      ? 'Nenhuma atividade encontrada com os filtros aplicados'
                      : 'Nenhuma atividade registrada'}
                  </Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
};

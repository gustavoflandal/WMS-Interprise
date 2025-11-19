import React, { useState } from 'react';
import {
  Container,
  Paper,
  Box,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Grid,
  CircularProgress,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Warehouse as WarehouseIcon,
  LocationOn as LocationIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { warehouseService, Warehouse, CreateWarehouseRequest, UpdateWarehouseRequest } from '../services/api/warehouseApi';
import { brazilianStates } from '../utils/brazilianStates';
import { formatCEP } from '../utils/formatters';

// ============================================================================
// Página de Cadastro de Armazéns
// ============================================================================

interface WarehouseFormData {
  nome: string;
  codigo: string;
  descricao: string;
  endereco: string;
  cidade: string;
  estado: string;
  cep: string;
  pais: string;
  latitude: string;
  longitude: string;
  totalPosicoes: string;
  capacidadePesoTotal: string;
  horarioAbertura: string;
  horarioFechamento: string;
  maxTrabalhadores: string;
  status: number;
}

const emptyForm: WarehouseFormData = {
  nome: '',
  codigo: '',
  descricao: '',
  endereco: '',
  cidade: '',
  estado: '',
  cep: '',
  pais: 'BRA',
  latitude: '',
  longitude: '',
  totalPosicoes: '',
  capacidadePesoTotal: '',
  horarioAbertura: '06:00',
  horarioFechamento: '18:00',
  maxTrabalhadores: '',
  status: 1,
};

export const WarehousePage: React.FC = () => {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<WarehouseFormData>(emptyForm);

  // Query para buscar todos os armazéns
  const { data: warehouses = [], isLoading, error } = useQuery({
    queryKey: ['warehouses'],
    queryFn: warehouseService.getAll,
  });

  // Mutation para criar armazém
  const createMutation = useMutation({
    mutationFn: (data: CreateWarehouseRequest) => warehouseService.create(data),
    onSuccess: () => {
      toast.success('Armazém criado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['warehouses'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao criar armazém');
    },
  });

  // Mutation para atualizar armazém
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateWarehouseRequest }) =>
      warehouseService.update(id, data),
    onSuccess: () => {
      toast.success('Armazém atualizado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['warehouses'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao atualizar armazém');
    },
  });

  // Mutation para deletar armazém
  const deleteMutation = useMutation({
    mutationFn: (id: string) => warehouseService.delete(id),
    onSuccess: () => {
      toast.success('Armazém excluído com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['warehouses'] });
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao excluir armazém');
    },
  });

  const handleOpenDialog = (warehouse?: Warehouse) => {
    if (warehouse) {
      setEditingId(warehouse.id);
      setFormData({
        nome: warehouse.nome,
        codigo: warehouse.codigo,
        descricao: warehouse.descricao || '',
        endereco: warehouse.endereco || '',
        cidade: warehouse.cidade || '',
        estado: warehouse.estado || '',
        cep: warehouse.cep || '',
        pais: warehouse.pais,
        latitude: warehouse.latitude?.toString() || '',
        longitude: warehouse.longitude?.toString() || '',
        totalPosicoes: warehouse.totalPosicoes?.toString() || '',
        capacidadePesoTotal: warehouse.capacidadePesoTotal?.toString() || '',
        horarioAbertura: warehouse.horarioAbertura?.substring(0, 5) || '06:00',
        horarioFechamento: warehouse.horarioFechamento?.substring(0, 5) || '18:00',
        maxTrabalhadores: warehouse.maxTrabalhadores?.toString() || '',
        status: warehouse.status,
      });
    } else {
      setEditingId(null);
      setFormData(emptyForm);
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingId(null);
    setFormData(emptyForm);
  };

  const handleInputChange = (field: keyof WarehouseFormData, value: string | number) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = () => {
    if (!formData.nome.trim() || !formData.codigo.trim()) {
      toast.error('Nome e código são obrigatórios');
      return;
    }

    const requestData = {
      nome: formData.nome,
      descricao: formData.descricao || undefined,
      endereco: formData.endereco || undefined,
      cidade: formData.cidade || undefined,
      estado: formData.estado || undefined,
      cep: formData.cep || undefined,
      pais: formData.pais || 'BRA',
      latitude: formData.latitude ? parseFloat(formData.latitude) : undefined,
      longitude: formData.longitude ? parseFloat(formData.longitude) : undefined,
      totalPosicoes: formData.totalPosicoes ? parseInt(formData.totalPosicoes) : undefined,
      capacidadePesoTotal: formData.capacidadePesoTotal ? parseFloat(formData.capacidadePesoTotal) : undefined,
      horarioAbertura: formData.horarioAbertura ? `${formData.horarioAbertura}:00` : undefined,
      horarioFechamento: formData.horarioFechamento ? `${formData.horarioFechamento}:00` : undefined,
      maxTrabalhadores: formData.maxTrabalhadores ? parseInt(formData.maxTrabalhadores) : undefined,
      status: formData.status,
    };

    if (editingId) {
      updateMutation.mutate({ id: editingId, data: requestData as UpdateWarehouseRequest });
    } else {
      createMutation.mutate({ ...requestData, codigo: formData.codigo } as CreateWarehouseRequest);
    }
  };

  const handleDelete = (id: string, nome: string) => {
    if (window.confirm(`Tem certeza que deseja excluir o armazém "${nome}"?`)) {
      deleteMutation.mutate(id);
    }
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 1: return 'success'; // Ativo
      case 2: return 'default'; // Inativo
      case 3: return 'warning'; // Em Manutenção
      default: return 'default';
    }
  };

  if (isLoading) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Alert severity="error">
          Erro ao carregar armazéns: {(error as any).message}
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper sx={{ p: 3 }}>
        {/* Header */}
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <WarehouseIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h5" component="h1">
              Gerenciamento de Armazéns
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
          >
            Novo Armazém
          </Button>
        </Box>

        {/* Tabela de Armazéns */}
        {warehouses.length === 0 ? (
          <Alert severity="info">
            Nenhum armazém cadastrado. Clique em "Novo Armazém" para começar.
          </Alert>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Código</TableCell>
                  <TableCell>Nome</TableCell>
                  <TableCell>Cidade/Estado</TableCell>
                  <TableCell>Capacidade</TableCell>
                  <TableCell align="center">Status</TableCell>
                  <TableCell align="center">Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {warehouses.map((warehouse) => (
                  <TableRow key={warehouse.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight="medium">
                        {warehouse.codigo}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{warehouse.nome}</Typography>
                      {warehouse.descricao && (
                        <Typography variant="caption" color="text.secondary">
                          {warehouse.descricao}
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell>
                      {warehouse.cidade && warehouse.estado ? (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                          <LocationIcon fontSize="small" color="action" />
                          <Typography variant="body2">
                            {warehouse.cidade}/{warehouse.estado}
                          </Typography>
                        </Box>
                      ) : (
                        <Typography variant="body2" color="text.secondary">
                          Não informado
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell>
                      {warehouse.totalPosicoes ? (
                        <Typography variant="body2">
                          {warehouse.totalPosicoes.toLocaleString('pt-BR')} posições
                        </Typography>
                      ) : (
                        <Typography variant="body2" color="text.secondary">
                          Não informado
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell align="center">
                      <Chip
                        label={warehouse.statusDescricao}
                        color={getStatusColor(warehouse.status)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="center">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={() => handleOpenDialog(warehouse)}
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDelete(warehouse.id, warehouse.nome)}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Dialog de Formulário */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingId ? 'Editar Armazém' : 'Novo Armazém'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            {/* Informações Básicas */}
            <Grid item xs={12}>
              <Typography variant="subtitle2" color="primary" gutterBottom>
                Informações Básicas
              </Typography>
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                required
                label="Nome do Armazém"
                value={formData.nome}
                onChange={(e) => handleInputChange('nome', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                required
                label="Código"
                value={formData.codigo}
                onChange={(e) => handleInputChange('codigo', e.target.value.toUpperCase())}
                disabled={!!editingId}
                helperText={editingId ? 'Código não pode ser alterado' : ''}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={2}
                label="Descrição"
                value={formData.descricao}
                onChange={(e) => handleInputChange('descricao', e.target.value)}
              />
            </Grid>

            {/* Localização */}
            <Grid item xs={12}>
              <Typography variant="subtitle2" color="primary" gutterBottom sx={{ mt: 2 }}>
                Localização
              </Typography>
            </Grid>
            <Grid item xs={12} md={3}>
              <TextField
                fullWidth
                label="CEP"
                value={formData.cep}
                onChange={(e) => handleInputChange('cep', formatCEP(e.target.value))}
                inputProps={{ maxLength: 10 }}
              />
            </Grid>
            <Grid item xs={12} md={9}>
              <TextField
                fullWidth
                label="Endereço"
                value={formData.endereco}
                onChange={(e) => handleInputChange('endereco', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={5}>
              <TextField
                fullWidth
                label="Cidade"
                value={formData.cidade}
                onChange={(e) => handleInputChange('cidade', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <FormControl fullWidth>
                <InputLabel>Estado</InputLabel>
                <Select
                  value={formData.estado}
                  label="Estado"
                  onChange={(e) => handleInputChange('estado', e.target.value)}
                >
                  {brazilianStates.map((state) => (
                    <MenuItem key={state.uf} value={state.uf}>
                      {state.uf} - {state.nome}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={2}>
              <TextField
                fullWidth
                label="País"
                value={formData.pais}
                onChange={(e) => handleInputChange('pais', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={2}>
              <TextField
                fullWidth
                label="Latitude"
                type="number"
                value={formData.latitude}
                onChange={(e) => handleInputChange('latitude', e.target.value)}
                inputProps={{ step: '0.0001' }}
              />
            </Grid>

            {/* Capacidade e Operação */}
            <Grid item xs={12}>
              <Typography variant="subtitle2" color="primary" gutterBottom sx={{ mt: 2 }}>
                Capacidade e Operação
              </Typography>
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Total de Posições"
                type="number"
                value={formData.totalPosicoes}
                onChange={(e) => handleInputChange('totalPosicoes', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Capacidade de Peso (kg)"
                type="number"
                value={formData.capacidadePesoTotal}
                onChange={(e) => handleInputChange('capacidadePesoTotal', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Máx. Trabalhadores"
                type="number"
                value={formData.maxTrabalhadores}
                onChange={(e) => handleInputChange('maxTrabalhadores', e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Horário Abertura"
                type="time"
                value={formData.horarioAbertura}
                onChange={(e) => handleInputChange('horarioAbertura', e.target.value)}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Horário Fechamento"
                type="time"
                value={formData.horarioFechamento}
                onChange={(e) => handleInputChange('horarioFechamento', e.target.value)}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} md={4}>
              <FormControl fullWidth>
                <InputLabel>Status</InputLabel>
                <Select
                  value={formData.status}
                  label="Status"
                  onChange={(e) => handleInputChange('status', e.target.value as number)}
                >
                  <MenuItem value={1}>Ativo</MenuItem>
                  <MenuItem value={2}>Inativo</MenuItem>
                  <MenuItem value={3}>Em Manutenção</MenuItem>
                </Select>
              </FormControl>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button
            variant="contained"
            onClick={handleSubmit}
            disabled={createMutation.isPending || updateMutation.isPending}
          >
            {createMutation.isPending || updateMutation.isPending ? (
              <CircularProgress size={24} />
            ) : editingId ? (
              'Salvar'
            ) : (
              'Criar'
            )}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

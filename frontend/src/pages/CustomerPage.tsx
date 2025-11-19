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
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { customerService, Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../services/api/customerApi';

// ============================================================================
// Página de Cadastro de Clientes
// ============================================================================

interface CustomerFormData {
  nome: string;
  tipo: string;
  numeroDocumento: string;
  email: string;
  telefone: string;
  status: number;
}

const emptyForm: CustomerFormData = {
  nome: '',
  tipo: 'PJ',
  numeroDocumento: '',
  email: '',
  telefone: '',
  status: 1,
};

export const CustomerPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<CustomerFormData>(emptyForm);
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);

  // Query para buscar todos os clientes
  const { data: customers = [], isLoading, error } = useQuery({
    queryKey: ['customers'],
    queryFn: customerService.getAll,
  });

  // Mutation para criar cliente
  const createMutation = useMutation({
    mutationFn: (data: CreateCustomerRequest) => customerService.create(data),
    onSuccess: () => {
      toast.success('Cliente criado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao criar cliente');
    },
  });

  // Mutation para atualizar cliente
  const updateMutation = useMutation({
    mutationFn: (data: { id: string; request: UpdateCustomerRequest }) =>
      customerService.update(data.id, data.request),
    onSuccess: () => {
      toast.success('Cliente atualizado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao atualizar cliente');
    },
  });

  // Mutation para deletar cliente
  const deleteMutation = useMutation({
    mutationFn: (id: string) => customerService.delete(id),
    onSuccess: () => {
      toast.success('Cliente deletado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      setConfirmDeleteId(null);
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao deletar cliente');
    },
  });

  const handleOpenDialog = (customer?: Customer) => {
    if (customer) {
      setEditingId(customer.id);
      setFormData({
        nome: customer.nome,
        tipo: customer.tipo,
        numeroDocumento: customer.numeroDocumento || '',
        email: customer.email || '',
        telefone: customer.telefone || '',
        status: customer.status,
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

  const handleChange = (field: keyof CustomerFormData) => (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | any
  ) => {
    setFormData({
      ...formData,
      [field]: event.target.value,
    });
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!formData.nome.trim()) {
      toast.error('Nome do cliente é obrigatório');
      return;
    }

    try {
      if (editingId) {
        await updateMutation.mutateAsync({
          id: editingId,
          request: {
            nome: formData.nome,
            tipo: formData.tipo,
            numeroDocumento: formData.numeroDocumento || undefined,
            email: formData.email || undefined,
            telefone: formData.telefone || undefined,
            status: formData.status,
          },
        });
      } else {
        await createMutation.mutateAsync({
          nome: formData.nome,
          tipo: formData.tipo,
          numeroDocumento: formData.numeroDocumento || undefined,
          email: formData.email || undefined,
          telefone: formData.telefone || undefined,
        });
      }
    } catch (error) {
      console.error('Erro ao salvar cliente:', error);
    }
  };

  const handleDelete = (id: string) => {
    setConfirmDeleteId(id);
  };

  const confirmDelete = async () => {
    if (confirmDeleteId) {
      await deleteMutation.mutateAsync(confirmDeleteId);
    }
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 1:
        return 'success';
      case 2:
        return 'default';
      case 3:
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusLabel = (status: number) => {
    switch (status) {
      case 1:
        return 'Ativo';
      case 2:
        return 'Inativo';
      case 3:
        return 'Bloqueado';
      default:
        return 'Desconhecido';
    }
  };

  if (isLoading) {
    return (
      <Container maxWidth="lg">
        <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 4 }}>
        {/* Header */}
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <h1 style={{ fontSize: '2.25rem', fontWeight: 'bold', margin: 0 }}>Clientes</h1>
            <Typography variant="body2" color="textSecondary">
              Gerenciar cadastro de clientes
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
          >
            Novo Cliente
          </Button>
        </Box>

        {/* Erro */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            Erro ao carregar clientes
          </Alert>
        )}

        {/* Tabela */}
        <TableContainer component={Paper}>
          <Table>
            <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
              <TableRow>
                <TableCell sx={{ fontWeight: 'bold' }}>Nome</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Tipo</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Documento</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Email</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Telefone</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Status</TableCell>
                <TableCell align="center" sx={{ fontWeight: 'bold' }}>
                  Ações
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {customers.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 3 }}>
                    <Typography color="textSecondary">Nenhum cliente cadastrado</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                customers.map((customer) => (
                  <TableRow key={customer.id} hover>
                    <TableCell>{customer.nome}</TableCell>
                    <TableCell>
                      <Chip
                        label={customer.tipo}
                        size="small"
                        color={customer.tipo === 'PJ' ? 'primary' : 'secondary'}
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>{customer.numeroDocumento || '-'}</TableCell>
                    <TableCell>{customer.email || '-'}</TableCell>
                    <TableCell>{customer.telefone || '-'}</TableCell>
                    <TableCell>
                      <Chip
                        label={getStatusLabel(customer.status)}
                        size="small"
                        color={getStatusColor(customer.status) as any}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={() => handleOpenDialog(customer)}
                        title="Editar"
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDelete(customer.id)}
                        title="Deletar"
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>

        {/* Diálogo de Formulário */}
        <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
          <DialogTitle>
            {editingId ? 'Editar Cliente' : 'Novo Cliente'}
          </DialogTitle>
          <DialogContent>
            <Box component="form" sx={{ pt: 2 }}>
              <Grid container spacing={3}>
                {/* Nome */}
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Nome *"
                    placeholder="Digite o nome do cliente"
                    value={formData.nome}
                    onChange={handleChange('nome')}
                    required
                  />
                </Grid>

                {/* Tipo */}
                <Grid item xs={12} sm={6}>
                  <FormControl fullWidth>
                    <InputLabel>Tipo *</InputLabel>
                    <Select
                      value={formData.tipo}
                      onChange={handleChange('tipo')}
                      label="Tipo *"
                    >
                      <MenuItem value="PJ">Pessoa Jurídica</MenuItem>
                      <MenuItem value="PF">Pessoa Física</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>

                {/* Status (só em edição) */}
                {editingId && (
                  <Grid item xs={12} sm={6}>
                    <FormControl fullWidth>
                      <InputLabel>Status</InputLabel>
                      <Select
                        value={formData.status}
                        onChange={handleChange('status')}
                        label="Status"
                      >
                        <MenuItem value={1}>Ativo</MenuItem>
                        <MenuItem value={2}>Inativo</MenuItem>
                        <MenuItem value={3}>Bloqueado</MenuItem>
                      </Select>
                    </FormControl>
                  </Grid>
                )}

                {/* Documento */}
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Número do Documento"
                    placeholder="CNPJ ou CPF"
                    value={formData.numeroDocumento}
                    onChange={handleChange('numeroDocumento')}
                  />
                </Grid>

                {/* Email */}
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Email"
                    type="email"
                    placeholder="exemplo@email.com"
                    value={formData.email}
                    onChange={handleChange('email')}
                  />
                </Grid>

                {/* Telefone */}
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Telefone"
                    placeholder="(11) 3456-7890"
                    value={formData.telefone}
                    onChange={handleChange('telefone')}
                  />
                </Grid>
              </Grid>
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancelar</Button>
            <Button
              onClick={(e) => {
                e.preventDefault();
                handleSubmit(e as any);
              }}
              variant="contained"
              color="primary"
              disabled={createMutation.isPending || updateMutation.isPending}
            >
              {createMutation.isPending || updateMutation.isPending ? (
                <CircularProgress size={24} />
              ) : (
                editingId ? 'Atualizar' : 'Salvar'
              )}
            </Button>
          </DialogActions>
        </Dialog>

        {/* Diálogo de Confirmação de Deleção */}
        <Dialog open={confirmDeleteId !== null} onClose={() => setConfirmDeleteId(null)}>
          <DialogTitle>Confirmar Exclusão</DialogTitle>
          <DialogContent>
            <Typography>
              Tem certeza que deseja excluir este cliente? Esta ação não pode ser desfeita.
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setConfirmDeleteId(null)}>Cancelar</Button>
            <Button
              onClick={confirmDelete}
              color="error"
              variant="contained"
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? <CircularProgress size={24} /> : 'Excluir'}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
};

export default CustomerPage;

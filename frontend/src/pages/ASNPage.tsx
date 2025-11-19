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
  Tabs,
  Tab,
  MenuItem,
  InputAdornment,
  Card,
  CardContent,
  Divider,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  LocalShipping as ShippingIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  Visibility as ViewIcon,
  PlayArrow as StartIcon,
  Inventory as InventoryIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { ptBR } from 'date-fns/locale';
import { format } from 'date-fns';
import asnService from '../services/api/asnApi';
import {
  ASN,
  CreateASNRequest,
  UpdateASNRequest,
  ASNStatus,
  ASNPriority,
  ASNStatusLabels,
  ASNPriorityLabels,
  ASNStatusColors,
  ASNPriorityColors,
  CreateASNItemRequest,
} from '../types/asn';

// ============================================================================
// Interface de abas
// ============================================================================

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`asn-tabpanel-${index}`}
      aria-labelledby={`asn-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

// ============================================================================
// Formulário de dados
// ============================================================================

interface ASNFormData {
  warehouseId: number;
  supplierId?: number;
  invoiceNumber?: string;
  scheduledArrivalDate: Date;
  priority: ASNPriority;
  externalReference?: string;
  originWarehouseId?: number;
  notes?: string;
  items: CreateASNItemRequest[];
}

const emptyForm: ASNFormData = {
  warehouseId: 1,
  scheduledArrivalDate: new Date(),
  priority: ASNPriority.Normal,
  items: [],
};

const emptyItem: CreateASNItemRequest = {
  productId: 0,
  expectedQuantity: 0,
  unit: 'UN',
};

// ============================================================================
// Página de Gerenciamento de ASN
// ============================================================================

export const ASNPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [openViewDialog, setOpenViewDialog] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [viewingAsn, setViewingAsn] = useState<ASN | null>(null);
  const [formData, setFormData] = useState<ASNFormData>(emptyForm);
  const [currentTab, setCurrentTab] = useState(0);
  const [searchFilter, setSearchFilter] = useState('');

  // Query para buscar todas as ASNs
  const { data: asns = [], isLoading, error } = useQuery({
    queryKey: ['asns'],
    queryFn: asnService.getAll,
  });

  // Mutation para criar ASN
  const createMutation = useMutation({
    mutationFn: (data: CreateASNRequest) => asnService.create(data),
    onSuccess: () => {
      toast.success('ASN criada com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
      handleCloseDialog();
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao criar ASN';
      toast.error(errorMessage);
    },
  });

  // Mutation para atualizar ASN
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateASNRequest }) =>
      asnService.update(id, data),
    onSuccess: () => {
      toast.success('ASN atualizada com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
      handleCloseDialog();
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao atualizar ASN';
      toast.error(errorMessage);
    },
  });

  // Mutation para deletar ASN
  const deleteMutation = useMutation({
    mutationFn: (id: string) => asnService.delete(id),
    onSuccess: () => {
      toast.success('ASN excluída com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao excluir ASN';
      toast.error(errorMessage);
    },
  });

  // Mutation para confirmar chegada
  const confirmArrivalMutation = useMutation({
    mutationFn: (id: string) => asnService.confirmArrival(id, { actualArrivalDate: new Date().toISOString() }),
    onSuccess: () => {
      toast.success('Chegada confirmada!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao confirmar chegada';
      toast.error(errorMessage);
    },
  });

  // Mutation para iniciar descarregamento
  const startUnloadingMutation = useMutation({
    mutationFn: (id: string) => asnService.startUnloading(id),
    onSuccess: () => {
      toast.success('Descarregamento iniciado!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao iniciar descarregamento';
      toast.error(errorMessage);
    },
  });

  // Mutation para cancelar ASN
  const cancelMutation = useMutation({
    mutationFn: (id: string) => asnService.cancel(id),
    onSuccess: () => {
      toast.success('ASN cancelada!');
      queryClient.invalidateQueries({ queryKey: ['asns'] });
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao cancelar ASN';
      toast.error(errorMessage);
    },
  });

  const handleOpenDialog = (asn?: ASN) => {
    if (asn) {
      setEditingId(asn.id);
      setFormData({
        warehouseId: asn.warehouseId,
        supplierId: asn.supplierId,
        invoiceNumber: asn.invoiceNumber || '',
        scheduledArrivalDate: new Date(asn.scheduledArrivalDate),
        priority: asn.priority,
        externalReference: asn.externalReference || '',
        originWarehouseId: asn.originWarehouseId,
        notes: asn.notes || '',
        items: [],
      });
    } else {
      setEditingId(null);
      setFormData(emptyForm);
    }
    setCurrentTab(0);
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingId(null);
    setFormData(emptyForm);
    setCurrentTab(0);
  };

  const handleOpenViewDialog = (asn: ASN) => {
    setViewingAsn(asn);
    setOpenViewDialog(true);
  };

  const handleCloseViewDialog = () => {
    setOpenViewDialog(false);
    setViewingAsn(null);
  };

  const handleInputChange = (field: keyof ASNFormData, value: unknown) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleAddItem = () => {
    setFormData((prev) => ({
      ...prev,
      items: [...prev.items, { ...emptyItem }],
    }));
  };

  const handleRemoveItem = (index: number) => {
    setFormData((prev) => ({
      ...prev,
      items: prev.items.filter((_, i) => i !== index),
    }));
  };

  const handleItemChange = (index: number, field: keyof CreateASNItemRequest, value: unknown) => {
    setFormData((prev) => ({
      ...prev,
      items: prev.items.map((item, i) =>
        i === index ? { ...item, [field]: value } : item
      ),
    }));
  };

  const handleSubmit = () => {
    if (!formData.warehouseId) {
      toast.error('Armazém é obrigatório');
      return;
    }

    if (formData.items.length === 0) {
      toast.error('Adicione pelo menos um item à ASN');
      return;
    }

    const requestData: CreateASNRequest = {
      warehouseId: formData.warehouseId,
      supplierId: formData.supplierId,
      invoiceNumber: formData.invoiceNumber,
      scheduledArrivalDate: formData.scheduledArrivalDate.toISOString(),
      priority: formData.priority,
      externalReference: formData.externalReference,
      originWarehouseId: formData.originWarehouseId,
      notes: formData.notes,
      items: formData.items,
    };

    if (editingId) {
      const updateData: UpdateASNRequest = {
        supplierId: formData.supplierId,
        invoiceNumber: formData.invoiceNumber,
        scheduledArrivalDate: formData.scheduledArrivalDate.toISOString(),
        priority: formData.priority,
        externalReference: formData.externalReference,
        notes: formData.notes,
      };
      updateMutation.mutate({ id: editingId, data: updateData });
    } else {
      createMutation.mutate(requestData);
    }
  };

  const handleDelete = (id: string, asnNumber: string) => {
    if (window.confirm(`Tem certeza que deseja excluir a ASN ${asnNumber}?`)) {
      deleteMutation.mutate(id);
    }
  };

  const handleConfirmArrival = (id: string) => {
    if (window.confirm('Confirmar chegada desta ASN?')) {
      confirmArrivalMutation.mutate(id);
    }
  };

  const handleStartUnloading = (id: string) => {
    if (window.confirm('Iniciar descarregamento?')) {
      startUnloadingMutation.mutate(id);
    }
  };

  const handleCancel = (id: string, asnNumber: string) => {
    if (window.confirm(`Tem certeza que deseja cancelar a ASN ${asnNumber}?`)) {
      cancelMutation.mutate(id);
    }
  };

  // Filtro de busca
  const filteredAsns = asns.filter((asn) =>
    asn.asnNumber.toLowerCase().includes(searchFilter.toLowerCase()) ||
    asn.invoiceNumber?.toLowerCase().includes(searchFilter.toLowerCase()) ||
    asn.externalReference?.toLowerCase().includes(searchFilter.toLowerCase())
  );

  if (isLoading) {
    return (
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error) {
    const errorMessage = (error as { message?: string })?.message || 'Erro desconhecido';
    return (
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Alert severity="error">
          Erro ao carregar ASNs: {errorMessage}
        </Alert>
      </Container>
    );
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ptBR}>
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Paper sx={{ p: 3 }}>
          {/* Header */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <ShippingIcon color="primary" sx={{ fontSize: 32 }} />
              <Typography variant="h5" component="h1">
                Avisos de Remessa (ASN)
              </Typography>
            </Box>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Nova ASN
            </Button>
          </Box>

          {/* Busca */}
          <Box sx={{ mb: 3 }}>
            <TextField
              fullWidth
              placeholder="Buscar por número ASN, NF ou referência externa..."
              value={searchFilter}
              onChange={(e) => setSearchFilter(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <InventoryIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Box>

          {/* Tabela de ASNs */}
          {filteredAsns.length === 0 ? (
            <Alert severity="info">
              {searchFilter ? 'Nenhuma ASN encontrada com os filtros aplicados.' : 'Nenhuma ASN cadastrada. Clique em &quot;Nova ASN&quot; para começar.'}
            </Alert>
          ) : (
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Número ASN</TableCell>
                    <TableCell>Nota Fiscal</TableCell>
                    <TableCell>Data Agendada</TableCell>
                    <TableCell align="center">Status</TableCell>
                    <TableCell align="center">Prioridade</TableCell>
                    <TableCell align="center">Itens Esperados</TableCell>
                    <TableCell align="center">Itens Recebidos</TableCell>
                    <TableCell align="center">Ações</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {filteredAsns.map((asn) => (
                    <TableRow key={asn.id} hover>
                      <TableCell>
                        <Typography variant="body2" fontWeight="medium">
                          {asn.asnNumber}
                        </Typography>
                      </TableCell>
                      <TableCell>{asn.invoiceNumber || '-'}</TableCell>
                      <TableCell>
                        {format(new Date(asn.scheduledArrivalDate), 'dd/MM/yyyy')}
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={ASNStatusLabels[asn.status]}
                          color={ASNStatusColors[asn.status]}
                          size="small"
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={ASNPriorityLabels[asn.priority]}
                          color={ASNPriorityColors[asn.priority]}
                          size="small"
                        />
                      </TableCell>
                      <TableCell align="center">{asn.expectedItemCount}</TableCell>
                      <TableCell align="center">{asn.receivedItemCount}</TableCell>
                      <TableCell align="center">
                        <IconButton
                          size="small"
                          color="info"
                          onClick={() => handleOpenViewDialog(asn)}
                          title="Visualizar"
                        >
                          <ViewIcon fontSize="small" />
                        </IconButton>
                        {asn.status === ASNStatus.Scheduled && (
                          <IconButton
                            size="small"
                            color="success"
                            onClick={() => handleConfirmArrival(asn.id)}
                            title="Confirmar Chegada"
                          >
                            <CheckCircleIcon fontSize="small" />
                          </IconButton>
                        )}
                        {asn.status === ASNStatus.Arrived && (
                          <IconButton
                            size="small"
                            color="warning"
                            onClick={() => handleStartUnloading(asn.id)}
                            title="Iniciar Descarregamento"
                          >
                            <StartIcon fontSize="small" />
                          </IconButton>
                        )}
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => handleOpenDialog(asn)}
                          disabled={asn.status !== ASNStatus.Scheduled}
                        >
                          <EditIcon fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleCancel(asn.id, asn.asnNumber)}
                          disabled={asn.status === ASNStatus.Cancelled || asn.status === ASNStatus.Received}
                        >
                          <CancelIcon fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleDelete(asn.id, asn.asnNumber)}
                          disabled={asn.status !== ASNStatus.Cancelled}
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
            {editingId ? 'Editar ASN' : 'Nova ASN'}
          </DialogTitle>
          <DialogContent>
            <Tabs value={currentTab} onChange={(_, newValue) => setCurrentTab(newValue)}>
              <Tab label="Dados Básicos" />
              <Tab label="Itens" />
            </Tabs>

            {/* Aba 1: Dados Básicos */}
            <TabPanel value={currentTab} index={0}>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    required
                    type="number"
                    label="ID Armazém"
                    value={formData.warehouseId}
                    onChange={(e) => handleInputChange('warehouseId', parseInt(e.target.value) || 0)}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    type="number"
                    label="ID Fornecedor"
                    value={formData.supplierId || ''}
                    onChange={(e) => handleInputChange('supplierId', parseInt(e.target.value) || undefined)}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Número Nota Fiscal"
                    value={formData.invoiceNumber || ''}
                    onChange={(e) => handleInputChange('invoiceNumber', e.target.value)}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <DatePicker
                    label="Data Agendada"
                    value={formData.scheduledArrivalDate}
                    onChange={(date) => handleInputChange('scheduledArrivalDate', date || new Date())}
                    slotProps={{ textField: { fullWidth: true, required: true } }}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    select
                    label="Prioridade"
                    value={formData.priority}
                    onChange={(e) => handleInputChange('priority', parseInt(e.target.value) as ASNPriority)}
                  >
                    {Object.entries(ASNPriorityLabels).map(([value, label]) => (
                      <MenuItem key={value} value={value}>
                        {label}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    label="Referência Externa"
                    value={formData.externalReference || ''}
                    onChange={(e) => handleInputChange('externalReference', e.target.value)}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    multiline
                    rows={3}
                    label="Observações"
                    value={formData.notes || ''}
                    onChange={(e) => handleInputChange('notes', e.target.value)}
                  />
                </Grid>
              </Grid>
            </TabPanel>

            {/* Aba 2: Itens */}
            <TabPanel value={currentTab} index={1}>
              <Box sx={{ mb: 2 }}>
                <Button
                  variant="outlined"
                  startIcon={<AddIcon />}
                  onClick={handleAddItem}
                >
                  Adicionar Item
                </Button>
              </Box>

              {formData.items.length === 0 ? (
                <Alert severity="info">
                  Nenhum item adicionado. Clique em &quot;Adicionar Item&quot; para começar.
                </Alert>
              ) : (
                formData.items.map((item, index) => (
                  <Card key={index} sx={{ mb: 2 }}>
                    <CardContent>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                        <Typography variant="h6">Item {index + 1}</Typography>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleRemoveItem(index)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      </Box>
                      <Grid container spacing={2}>
                        <Grid item xs={12} sm={4}>
                          <TextField
                            fullWidth
                            required
                            type="number"
                            label="ID Produto"
                            value={item.productId || ''}
                            onChange={(e) =>
                              handleItemChange(index, 'productId', parseInt(e.target.value) || 0)
                            }
                          />
                        </Grid>
                        <Grid item xs={12} sm={4}>
                          <TextField
                            fullWidth
                            required
                            type="number"
                            label="Quantidade Esperada"
                            value={item.expectedQuantity || ''}
                            onChange={(e) =>
                              handleItemChange(index, 'expectedQuantity', parseInt(e.target.value) || 0)
                            }
                          />
                        </Grid>
                        <Grid item xs={12} sm={4}>
                          <TextField
                            fullWidth
                            label="Unidade"
                            value={item.unit || 'UN'}
                            onChange={(e) => handleItemChange(index, 'unit', e.target.value)}
                          />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                          <TextField
                            fullWidth
                            type="number"
                            label="Peso Esperado (kg)"
                            value={item.expectedWeight || ''}
                            onChange={(e) =>
                              handleItemChange(index, 'expectedWeight', parseFloat(e.target.value) || undefined)
                            }
                          />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                          <TextField
                            fullWidth
                            type="number"
                            label="Volume Esperado (m³)"
                            value={item.expectedVolume || ''}
                            onChange={(e) =>
                              handleItemChange(index, 'expectedVolume', parseFloat(e.target.value) || undefined)
                            }
                          />
                        </Grid>
                        <Grid item xs={12}>
                          <TextField
                            fullWidth
                            multiline
                            rows={2}
                            label="Observações do Item"
                            value={item.notes || ''}
                            onChange={(e) => handleItemChange(index, 'notes', e.target.value)}
                          />
                        </Grid>
                      </Grid>
                    </CardContent>
                  </Card>
                ))
              )}
            </TabPanel>
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

        {/* Dialog de Visualização */}
        <Dialog open={openViewDialog} onClose={handleCloseViewDialog} maxWidth="lg" fullWidth>
          <DialogTitle>Detalhes da ASN - {viewingAsn?.asnNumber}</DialogTitle>
          <DialogContent>
            {viewingAsn && (
              <Grid container spacing={3}>
                <Grid item xs={12}>
                  <Card>
                    <CardContent>
                      <Typography variant="h6" gutterBottom>
                        Informações Gerais
                      </Typography>
                      <Divider sx={{ mb: 2 }} />
                      <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                          <Typography variant="body2" color="textSecondary">
                            Status
                          </Typography>
                          <Chip
                            label={ASNStatusLabels[viewingAsn.status]}
                            color={ASNStatusColors[viewingAsn.status]}
                            size="small"
                          />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                          <Typography variant="body2" color="textSecondary">
                            Prioridade
                          </Typography>
                          <Chip
                            label={ASNPriorityLabels[viewingAsn.priority]}
                            color={ASNPriorityColors[viewingAsn.priority]}
                            size="small"
                          />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                          <Typography variant="body2" color="textSecondary">
                            Data Agendada
                          </Typography>
                          <Typography variant="body1">
                            {format(new Date(viewingAsn.scheduledArrivalDate), 'dd/MM/yyyy HH:mm')}
                          </Typography>
                        </Grid>
                        {viewingAsn.actualArrivalDate && (
                          <Grid item xs={12} sm={6}>
                            <Typography variant="body2" color="textSecondary">
                              Data de Chegada
                            </Typography>
                            <Typography variant="body1">
                              {format(new Date(viewingAsn.actualArrivalDate), 'dd/MM/yyyy HH:mm')}
                            </Typography>
                          </Grid>
                        )}
                        <Grid item xs={12} sm={6}>
                          <Typography variant="body2" color="textSecondary">
                            Itens Esperados
                          </Typography>
                          <Typography variant="body1">{viewingAsn.expectedItemCount}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                          <Typography variant="body2" color="textSecondary">
                            Itens Recebidos
                          </Typography>
                          <Typography variant="body1">{viewingAsn.receivedItemCount}</Typography>
                        </Grid>
                        {viewingAsn.notes && (
                          <Grid item xs={12}>
                            <Typography variant="body2" color="textSecondary">
                              Observações
                            </Typography>
                            <Typography variant="body1">{viewingAsn.notes}</Typography>
                          </Grid>
                        )}
                      </Grid>
                    </CardContent>
                  </Card>
                </Grid>

                <Grid item xs={12}>
                  <Card>
                    <CardContent>
                      <Typography variant="h6" gutterBottom>
                        Itens da ASN
                      </Typography>
                      <Divider sx={{ mb: 2 }} />
                      {viewingAsn.items.length === 0 ? (
                        <Alert severity="info">Nenhum item cadastrado nesta ASN.</Alert>
                      ) : (
                        <TableContainer>
                          <Table size="small">
                            <TableHead>
                              <TableRow>
                                <TableCell>Produto ID</TableCell>
                                <TableCell align="right">Qtd Esperada</TableCell>
                                <TableCell align="right">Qtd Recebida</TableCell>
                                <TableCell>Unidade</TableCell>
                                <TableCell>Lote</TableCell>
                                <TableCell>Status</TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {viewingAsn.items.map((item) => (
                                <TableRow key={item.id}>
                                  <TableCell>{item.productId}</TableCell>
                                  <TableCell align="right">{item.expectedQuantity}</TableCell>
                                  <TableCell align="right">{item.receivedQuantity}</TableCell>
                                  <TableCell>{item.unit}</TableCell>
                                  <TableCell>{item.lotNumber || '-'}</TableCell>
                                  <TableCell>
                                    <Typography
                                      variant="caption"
                                      color={item.isConformed ? 'success.main' : 'warning.main'}
                                    >
                                      {item.isConformed ? 'Conforme' : 'Pendente'}
                                    </Typography>
                                  </TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </TableContainer>
                      )}
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseViewDialog}>Fechar</Button>
          </DialogActions>
        </Dialog>
      </Container>
    </LocalizationProvider>
  );
};

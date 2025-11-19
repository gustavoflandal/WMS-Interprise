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
  FormControlLabel,
  Checkbox,
  Tabs,
  Tab,
  InputAdornment,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Inventory as InventoryIcon,
  Search as SearchIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { productService } from '../services/api/productApi';
import {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
  ProductCategory,
  ProductType,
  StorageZone,
  ABCClassification,
  ProductCategoryLabels,
  ProductTypeLabels,
  StorageZoneLabels,
  ABCClassificationLabels,
} from '../types/product';

// ============================================================================
// Página de Gerenciamento de Produtos
// ============================================================================

interface ProductFormData {
  sku: string;
  name: string;
  category: number;
  type: number;
  unitWeight: string;
  unitVolume: string;
  defaultStorageZone: number;
  requiresLotTracking: boolean;
  requiresSerialNumber: boolean;
  shelfLifeDays: string;
  minStorageTemperature: string;
  maxStorageTemperature: string;
  minStorageHumidity: string;
  maxStorageHumidity: string;
  isFlammable: boolean;
  isDangerous: boolean;
  isPharmaceutical: boolean;
  abcClassification: number | '';
  unitCost: string;
  unitPrice: string;
}

const emptyForm: ProductFormData = {
  sku: '',
  name: '',
  category: ProductCategory.Dry,
  type: ProductType.Commodity,
  unitWeight: '',
  unitVolume: '',
  defaultStorageZone: StorageZone.Reserve,
  requiresLotTracking: false,
  requiresSerialNumber: false,
  shelfLifeDays: '',
  minStorageTemperature: '',
  maxStorageTemperature: '',
  minStorageHumidity: '',
  maxStorageHumidity: '',
  isFlammable: false,
  isDangerous: false,
  isPharmaceutical: false,
  abcClassification: '',
  unitCost: '',
  unitPrice: '',
};

export const ProductsPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState<ProductFormData>(emptyForm);
  const [tabValue, setTabValue] = useState(0);
  const [searchTerm, setSearchTerm] = useState('');

  // Query para buscar todos os produtos
  const { data: products = [], isLoading, error } = useQuery({
    queryKey: ['products'],
    queryFn: productService.getAll,
  });

  // Mutation para criar produto
  const createMutation = useMutation({
    mutationFn: (data: CreateProductRequest) => productService.create(data),
    onSuccess: () => {
      toast.success('Produto criado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['products'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao criar produto');
    },
  });

  // Mutation para atualizar produto
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProductRequest }) =>
      productService.update(id, data),
    onSuccess: () => {
      toast.success('Produto atualizado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['products'] });
      handleCloseDialog();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao atualizar produto');
    },
  });

  // Mutation para deletar produto
  const deleteMutation = useMutation({
    mutationFn: (id: string) => productService.delete(id),
    onSuccess: () => {
      toast.success('Produto excluído com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao excluir produto');
    },
  });

  const handleOpenDialog = (product?: Product) => {
    if (product) {
      setEditingId(product.id);
      setFormData({
        sku: product.sku,
        name: product.name,
        category: product.category,
        type: product.type,
        unitWeight: product.unitWeight.toString(),
        unitVolume: product.unitVolume.toString(),
        defaultStorageZone: product.defaultStorageZone,
        requiresLotTracking: product.requiresLotTracking,
        requiresSerialNumber: product.requiresSerialNumber,
        shelfLifeDays: product.shelfLifeDays?.toString() || '',
        minStorageTemperature: product.minStorageTemperature?.toString() || '',
        maxStorageTemperature: product.maxStorageTemperature?.toString() || '',
        minStorageHumidity: product.minStorageHumidity?.toString() || '',
        maxStorageHumidity: product.maxStorageHumidity?.toString() || '',
        isFlammable: product.isFlammable,
        isDangerous: product.isDangerous,
        isPharmaceutical: product.isPharmaceutical,
        abcClassification: product.abcClassification || '',
        unitCost: product.unitCost?.toString() || '',
        unitPrice: product.unitPrice?.toString() || '',
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

  const handleInputChange = (field: keyof ProductFormData, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = () => {
    if (!formData.sku.trim() || !formData.name.trim()) {
      toast.error('SKU e Nome são obrigatórios');
      return;
    }

    if (!formData.unitWeight || !formData.unitVolume) {
      toast.error('Peso e Volume são obrigatórios');
      return;
    }

    const requestData: CreateProductRequest | UpdateProductRequest = {
      sku: formData.sku,
      name: formData.name,
      category: formData.category,
      type: formData.type,
      unitWeight: parseFloat(formData.unitWeight),
      unitVolume: parseFloat(formData.unitVolume),
      defaultStorageZone: formData.defaultStorageZone,
      requiresLotTracking: formData.requiresLotTracking,
      requiresSerialNumber: formData.requiresSerialNumber,
      shelfLifeDays: formData.shelfLifeDays ? parseInt(formData.shelfLifeDays) : undefined,
      minStorageTemperature: formData.minStorageTemperature ? parseFloat(formData.minStorageTemperature) : undefined,
      maxStorageTemperature: formData.maxStorageTemperature ? parseFloat(formData.maxStorageTemperature) : undefined,
      minStorageHumidity: formData.minStorageHumidity ? parseFloat(formData.minStorageHumidity) : undefined,
      maxStorageHumidity: formData.maxStorageHumidity ? parseFloat(formData.maxStorageHumidity) : undefined,
      isFlammable: formData.isFlammable,
      isDangerous: formData.isDangerous,
      isPharmaceutical: formData.isPharmaceutical,
      abcClassification: formData.abcClassification !== '' ? Number(formData.abcClassification) : undefined,
      unitCost: formData.unitCost ? parseFloat(formData.unitCost) : undefined,
      unitPrice: formData.unitPrice ? parseFloat(formData.unitPrice) : undefined,
    };

    if (editingId) {
      updateMutation.mutate({ id: editingId, data: requestData as UpdateProductRequest });
    } else {
      createMutation.mutate(requestData as CreateProductRequest);
    }
  };

  const handleDelete = (id: string, name: string) => {
    if (window.confirm(`Tem certeza que deseja excluir o produto "${name}"?`)) {
      deleteMutation.mutate(id);
    }
  };

  const getCategoryColor = (category: ProductCategory) => {
    const colors: Record<ProductCategory, "default" | "primary" | "secondary" | "error" | "info" | "success" | "warning"> = {
      [ProductCategory.Dry]: 'default',
      [ProductCategory.Refrigerated]: 'info',
      [ProductCategory.Frozen]: 'primary',
      [ProductCategory.Perishable]: 'warning',
      [ProductCategory.Controlled]: 'error',
      [ProductCategory.BulkVolume]: 'secondary',
      [ProductCategory.SmallVolume]: 'default',
      [ProductCategory.HighValue]: 'success',
    };
    return colors[category];
  };

  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    product.sku.toLowerCase().includes(searchTerm.toLowerCase())
  );

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
          Erro ao carregar produtos: {(error as any).message}
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
            <InventoryIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h5" component="h1">
              Gerenciamento de Produtos
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
          >
            Novo Produto
          </Button>
        </Box>

        {/* Busca */}
        <Box sx={{ mb: 3 }}>
          <TextField
            fullWidth
            placeholder="Buscar por nome ou SKU..."
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
        </Box>

        {/* Tabela de Produtos */}
        {filteredProducts.length === 0 ? (
          <Alert severity="info">
            {searchTerm ? 'Nenhum produto encontrado.' : 'Nenhum produto cadastrado. Clique em "Novo Produto" para começar.'}
          </Alert>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>SKU</TableCell>
                  <TableCell>Nome</TableCell>
                  <TableCell>Categoria</TableCell>
                  <TableCell>Tipo</TableCell>
                  <TableCell align="right">Peso (kg)</TableCell>
                  <TableCell align="right">Volume (m³)</TableCell>
                  <TableCell align="center">Status</TableCell>
                  <TableCell align="center">Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredProducts.map((product) => (
                  <TableRow key={product.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight="medium">
                        {product.sku}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{product.name}</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={ProductCategoryLabels[product.category]}
                        color={getCategoryColor(product.category)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">
                        {ProductTypeLabels[product.type]}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="body2">
                        {product.unitWeight.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="body2">
                        {product.unitVolume.toFixed(4)}
                      </Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Chip
                        label={product.isActive ? 'Ativo' : 'Inativo'}
                        color={product.isActive ? 'success' : 'default'}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="center">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={() => handleOpenDialog(product)}
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDelete(product.id, product.name)}
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
          {editingId ? 'Editar Produto' : 'Novo Produto'}
        </DialogTitle>
        <DialogContent>
          <Tabs value={tabValue} onChange={(_, newValue) => setTabValue(newValue)} sx={{ mb: 2 }}>
            <Tab label="Informações Básicas" />
            <Tab label="Armazenamento" />
            <Tab label="Características" />
            <Tab label="Custos" />
          </Tabs>

          {/* Tab 0: Informações Básicas */}
          {tabValue === 0 && (
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="SKU"
                  value={formData.sku}
                  onChange={(e) => handleInputChange('sku', e.target.value.toUpperCase())}
                  disabled={!!editingId}
                  helperText={editingId ? 'SKU não pode ser alterado' : ''}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="Nome do Produto"
                  value={formData.name}
                  onChange={(e) => handleInputChange('name', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControl fullWidth required>
                  <InputLabel>Categoria</InputLabel>
                  <Select
                    value={formData.category}
                    label="Categoria"
                    onChange={(e) => handleInputChange('category', e.target.value)}
                  >
                    {Object.entries(ProductCategoryLabels).map(([key, value]) => (
                      <MenuItem key={key} value={parseInt(key)}>
                        {value}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControl fullWidth required>
                  <InputLabel>Tipo</InputLabel>
                  <Select
                    value={formData.type}
                    label="Tipo"
                    onChange={(e) => handleInputChange('type', e.target.value)}
                  >
                    {Object.entries(ProductTypeLabels).map(([key, value]) => (
                      <MenuItem key={key} value={parseInt(key)}>
                        {value}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="Peso Unitário (kg)"
                  type="number"
                  value={formData.unitWeight}
                  onChange={(e) => handleInputChange('unitWeight', e.target.value)}
                  inputProps={{ step: '0.01', min: '0.01' }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="Volume Unitário (m³)"
                  type="number"
                  value={formData.unitVolume}
                  onChange={(e) => handleInputChange('unitVolume', e.target.value)}
                  inputProps={{ step: '0.0001', min: '0.0001' }}
                />
              </Grid>
            </Grid>
          )}

          {/* Tab 1: Armazenamento */}
          {tabValue === 1 && (
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <FormControl fullWidth required>
                  <InputLabel>Zona de Armazenamento Padrão</InputLabel>
                  <Select
                    value={formData.defaultStorageZone}
                    label="Zona de Armazenamento Padrão"
                    onChange={(e) => handleInputChange('defaultStorageZone', e.target.value)}
                  >
                    {Object.entries(StorageZoneLabels).map(([key, value]) => (
                      <MenuItem key={key} value={parseInt(key)}>
                        {value}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControl fullWidth>
                  <InputLabel>Classificação ABC</InputLabel>
                  <Select
                    value={formData.abcClassification}
                    label="Classificação ABC"
                    onChange={(e) => handleInputChange('abcClassification', e.target.value)}
                  >
                    <MenuItem value="">Nenhuma</MenuItem>
                    {Object.entries(ABCClassificationLabels).map(([key, value]) => (
                      <MenuItem key={key} value={parseInt(key)}>
                        {value}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Temperatura Mínima (°C)"
                  type="number"
                  value={formData.minStorageTemperature}
                  onChange={(e) => handleInputChange('minStorageTemperature', e.target.value)}
                  inputProps={{ step: '0.1' }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Temperatura Máxima (°C)"
                  type="number"
                  value={formData.maxStorageTemperature}
                  onChange={(e) => handleInputChange('maxStorageTemperature', e.target.value)}
                  inputProps={{ step: '0.1' }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Umidade Mínima (%)"
                  type="number"
                  value={formData.minStorageHumidity}
                  onChange={(e) => handleInputChange('minStorageHumidity', e.target.value)}
                  inputProps={{ min: '0', max: '100' }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Umidade Máxima (%)"
                  type="number"
                  value={formData.maxStorageHumidity}
                  onChange={(e) => handleInputChange('maxStorageHumidity', e.target.value)}
                  inputProps={{ min: '0', max: '100' }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Vida Útil (dias)"
                  type="number"
                  value={formData.shelfLifeDays}
                  onChange={(e) => handleInputChange('shelfLifeDays', e.target.value)}
                  inputProps={{ min: '1' }}
                />
              </Grid>
            </Grid>
          )}

          {/* Tab 2: Características */}
          {tabValue === 2 && (
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formData.requiresLotTracking}
                      onChange={(e) => handleInputChange('requiresLotTracking', e.target.checked)}
                    />
                  }
                  label="Requer Rastreamento de Lote"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formData.requiresSerialNumber}
                      onChange={(e) => handleInputChange('requiresSerialNumber', e.target.checked)}
                    />
                  }
                  label="Requer Número de Série"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formData.isFlammable}
                      onChange={(e) => handleInputChange('isFlammable', e.target.checked)}
                    />
                  }
                  label="Inflamável"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formData.isDangerous}
                      onChange={(e) => handleInputChange('isDangerous', e.target.checked)}
                    />
                  }
                  label="Perigoso"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formData.isPharmaceutical}
                      onChange={(e) => handleInputChange('isPharmaceutical', e.target.checked)}
                    />
                  }
                  label="Produto Farmacêutico"
                />
              </Grid>
            </Grid>
          )}

          {/* Tab 3: Custos */}
          {tabValue === 3 && (
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Custo Unitário (R$)"
                  type="number"
                  value={formData.unitCost}
                  onChange={(e) => handleInputChange('unitCost', e.target.value)}
                  inputProps={{ step: '0.01', min: '0.01' }}
                  InputProps={{
                    startAdornment: <InputAdornment position="start">R$</InputAdornment>,
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Preço de Venda (R$)"
                  type="number"
                  value={formData.unitPrice}
                  onChange={(e) => handleInputChange('unitPrice', e.target.value)}
                  inputProps={{ step: '0.01', min: '0.01' }}
                  InputProps={{
                    startAdornment: <InputAdornment position="start">R$</InputAdornment>,
                  }}
                />
              </Grid>
            </Grid>
          )}
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

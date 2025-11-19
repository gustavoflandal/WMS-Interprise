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
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Checkbox,
  Divider,
  Card,
  CardContent,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Shield as ShieldIcon,
  People as PeopleIcon,
  Security as SecurityIcon,
  Check as CheckIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { roleService, Role, CreateRoleRequest, UpdateRoleRequest } from '../services/api/roleApi';
import { permissionService } from '../services/api/permissionApi';

// ============================================================================
// Página de Gerenciamento de Papéis/Funções
// ============================================================================

interface RoleFormData {
  name: string;
  description: string;
}

const emptyForm: RoleFormData = {
  name: '',
  description: '',
};

export const RolesPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [openDialog, setOpenDialog] = useState(false);
  const [openPermissionsDialog, setOpenPermissionsDialog] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [selectedRoleForPermissions, setSelectedRoleForPermissions] = useState<Role | null>(null);
  const [formData, setFormData] = useState<RoleFormData>(emptyForm);
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);

  // Query para buscar todos os papéis
  const { data: roles = [], isLoading: rolesLoading, error: rolesError } = useQuery({
    queryKey: ['roles'],
    queryFn: roleService.getAll,
  });

  // Query para buscar todas as permissões
  const { data: permissionsByModule, isLoading: permissionsLoading } = useQuery({
    queryKey: ['permissions-by-module'],
    queryFn: permissionService.getByModule,
  });

  // Mutation para criar papel
  const createMutation = useMutation({
    mutationFn: (data: CreateRoleRequest) => roleService.create(data),
    onSuccess: () => {
      toast.success('Papel criado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      handleCloseDialog();
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao criar papel';
      toast.error(errorMessage);
    },
  });

  // Mutation para atualizar papel
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateRoleRequest }) =>
      roleService.update(id, data),
    onSuccess: () => {
      toast.success('Papel atualizado com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      handleCloseDialog();
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao atualizar papel';
      toast.error(errorMessage);
    },
  });

  // Mutation para deletar papel
  const deleteMutation = useMutation({
    mutationFn: (id: string) => roleService.delete(id),
    onSuccess: () => {
      toast.success('Papel excluído com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao excluir papel';
      toast.error(errorMessage);
    },
  });

  // Mutation para atribuir permissões
  const assignPermissionsMutation = useMutation({
    mutationFn: ({ id, permissionIds }: { id: string; permissionIds: string[] }) =>
      roleService.assignPermissions(id, { permissionIds }),
    onSuccess: () => {
      toast.success('Permissões atualizadas com sucesso!');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      handleClosePermissionsDialog();
    },
    onError: (error: unknown) => {
      const errorMessage = (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Erro ao atribuir permissões';
      toast.error(errorMessage);
    },
  });

  const handleOpenDialog = (role?: Role) => {
    if (role) {
      setEditingId(role.id);
      setFormData({
        name: role.name,
        description: role.description,
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

  const handleOpenPermissionsDialog = (role: Role) => {
    setSelectedRoleForPermissions(role);
    setSelectedPermissions(role.permissions.map(p => p.id));
    setOpenPermissionsDialog(true);
  };

  const handleClosePermissionsDialog = () => {
    setOpenPermissionsDialog(false);
    setSelectedRoleForPermissions(null);
    setSelectedPermissions([]);
  };

  const handleInputChange = (field: keyof RoleFormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleTogglePermission = (permissionId: string) => {
    setSelectedPermissions((prev) =>
      prev.includes(permissionId)
        ? prev.filter((id) => id !== permissionId)
        : [...prev, permissionId]
    );
  };

  const handleSubmit = () => {
    if (!formData.name.trim()) {
      toast.error('Nome é obrigatório');
      return;
    }

    if (!formData.description.trim()) {
      toast.error('Descrição é obrigatória');
      return;
    }

    const requestData: CreateRoleRequest | UpdateRoleRequest = {
      name: formData.name,
      description: formData.description,
    };

    if (editingId) {
      updateMutation.mutate({ id: editingId, data: requestData as UpdateRoleRequest });
    } else {
      createMutation.mutate(requestData as CreateRoleRequest);
    }
  };

  const handleSavePermissions = () => {
    if (selectedRoleForPermissions) {
      assignPermissionsMutation.mutate({
        id: selectedRoleForPermissions.id,
        permissionIds: selectedPermissions,
      });
    }
  };

  const handleDelete = (id: string, name: string, isSystemRole: boolean) => {
    if (isSystemRole) {
      toast.error('Não é possível excluir papéis do sistema');
      return;
    }
    if (window.confirm(`Tem certeza que deseja excluir o papel "${name}"?`)) {
      deleteMutation.mutate(id);
    }
  };

  if (rolesLoading) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (rolesError) {
    const errorMessage = (rolesError as { message?: string })?.message || 'Erro desconhecido';
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Alert severity="error">
          Erro ao carregar papéis: {errorMessage}
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
            <ShieldIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h5" component="h1">
              Gerenciamento de Papéis
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
          >
            Novo Papel
          </Button>
        </Box>

        {/* Tabela de Papéis */}
        {roles.length === 0 ? (
          <Alert severity="info">
            Nenhum papel cadastrado. Clique em &quot;Novo Papel&quot; para começar.
          </Alert>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Nome</TableCell>
                  <TableCell>Descrição</TableCell>
                  <TableCell align="center">Permissões</TableCell>
                  <TableCell align="center">Usuários</TableCell>
                  <TableCell align="center">Tipo</TableCell>
                  <TableCell align="center">Ações</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {roles.map((role) => (
                  <TableRow key={role.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight="medium">
                        {role.name}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">
                        {role.description}
                      </Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Chip
                        label={role.permissions.length}
                        color="primary"
                        size="small"
                        onClick={() => handleOpenPermissionsDialog(role)}
                        icon={<SecurityIcon />}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <Chip
                        label={role.userCount}
                        color="secondary"
                        size="small"
                        icon={<PeopleIcon />}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <Chip
                        label={role.isSystemRole ? 'Sistema' : 'Customizado'}
                        color={role.isSystemRole ? 'default' : 'info'}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="center">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={() => handleOpenDialog(role)}
                        disabled={role.isSystemRole}
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDelete(role.id, role.name, role.isSystemRole)}
                        disabled={role.isSystemRole || role.userCount > 0}
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
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {editingId ? 'Editar Papel' : 'Novo Papel'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Nome do Papel"
                value={formData.name}
                onChange={(e) => handleInputChange('name', e.target.value)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                multiline
                rows={3}
                label="Descrição"
                value={formData.description}
                onChange={(e) => handleInputChange('description', e.target.value)}
              />
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

      {/* Dialog de Permissões */}
      <Dialog
        open={openPermissionsDialog}
        onClose={handleClosePermissionsDialog}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          Gerenciar Permissões - {selectedRoleForPermissions?.name}
        </DialogTitle>
        <DialogContent>
          {permissionsLoading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
              <CircularProgress />
            </Box>
          ) : (
            <Box sx={{ mt: 2 }}>
              {Object.entries(permissionsByModule || {}).map(([module, permissions]) => (
                <Card key={module} sx={{ mb: 2 }}>
                  <CardContent>
                    <Typography variant="h6" gutterBottom color="primary">
                      {module}
                    </Typography>
                    <Divider sx={{ mb: 1 }} />
                    <List dense>
                      {permissions.map((permission) => (
                        <ListItem
                          key={permission.id}
                          button
                          onClick={() => handleTogglePermission(permission.id)}
                        >
                          <ListItemIcon>
                            <Checkbox
                              edge="start"
                              checked={selectedPermissions.includes(permission.id)}
                              tabIndex={-1}
                              disableRipple
                            />
                          </ListItemIcon>
                          <ListItemText
                            primary={permission.name}
                            secondary={permission.description}
                          />
                        </ListItem>
                      ))}
                    </List>
                  </CardContent>
                </Card>
              ))}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClosePermissionsDialog}>Cancelar</Button>
          <Button
            variant="contained"
            onClick={handleSavePermissions}
            disabled={assignPermissionsMutation.isPending}
            startIcon={<CheckIcon />}
          >
            {assignPermissionsMutation.isPending ? (
              <CircularProgress size={24} />
            ) : (
              'Salvar Permissões'
            )}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

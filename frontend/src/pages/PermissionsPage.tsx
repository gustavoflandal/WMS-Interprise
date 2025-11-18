import React, { useState } from 'react';
import {
  Container,
  Paper,
  Box,
  Typography,
  Tabs,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Chip,
  Grid,
  Checkbox,
  FormControlLabel,
  FormGroup,
  Alert,
  CircularProgress,
  Card,
  CardContent,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  ExpandMore as ExpandMoreIcon,
  Security as SecurityIcon,
  Group as GroupIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { roleService, Role, CreateRoleRequest, UpdateRoleRequest } from '../services/api/roleApi';
import { permissionService, Permission } from '../services/api/permissionApi';
import { toast } from 'react-toastify';

// ============================================================================
// Interfaces
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
      id={`permissions-tabpanel-${index}`}
      aria-labelledby={`permissions-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

// ============================================================================
// Modal de Role
// ============================================================================

interface RoleModalProps {
  open: boolean;
  onClose: () => void;
  role?: Role;
  permissions: Permission[];
}

const RoleModal: React.FC<RoleModalProps> = ({ open, onClose, role, permissions }) => {
  const queryClient = useQueryClient();
  const [formData, setFormData] = useState({
    name: role?.name || '',
    description: role?.description || '',
  });
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>(
    role?.permissions.map(p => p.id) || []
  );

  React.useEffect(() => {
    if (role) {
      setFormData({
        name: role.name,
        description: role.description,
      });
      setSelectedPermissions(role.permissions.map(p => p.id));
    } else {
      setFormData({ name: '', description: '' });
      setSelectedPermissions([]);
    }
  }, [role, open]);

  const createMutation = useMutation({
    mutationFn: (data: CreateRoleRequest) => roleService.create(data),
    onSuccess: async (newRole) => {
      if (selectedPermissions.length > 0) {
        await roleService.assignPermissions(newRole.id, { permissionIds: selectedPermissions });
      }
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      toast.success('Role criado com sucesso!');
      onClose();
    },
    onError: () => {
      toast.error('Erro ao criar role');
    },
  });

  const updateMutation = useMutation({
    mutationFn: async (data: UpdateRoleRequest) => {
      if (!role) throw new Error('Role não encontrado');
      await roleService.update(role.id, data);
      await roleService.assignPermissions(role.id, { permissionIds: selectedPermissions });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      toast.success('Role atualizado com sucesso!');
      onClose();
    },
    onError: () => {
      toast.error('Erro ao atualizar role');
    },
  });

  const handleSubmit = () => {
    if (role) {
      updateMutation.mutate(formData);
    } else {
      createMutation.mutate(formData);
    }
  };

  const handlePermissionToggle = (permissionId: string) => {
    setSelectedPermissions(prev =>
      prev.includes(permissionId)
        ? prev.filter(id => id !== permissionId)
        : [...prev, permissionId]
    );
  };

  const groupedPermissions = permissions.reduce((acc, permission) => {
    if (!acc[permission.module]) {
      acc[permission.module] = [];
    }
    acc[permission.module].push(permission);
    return acc;
  }, {} as Record<string, Permission[]>);

  const isLoading = createMutation.isPending || updateMutation.isPending;

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        {role ? 'Editar Role' : 'Novo Role'}
      </DialogTitle>
      <DialogContent dividers>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
          <TextField
            fullWidth
            label="Nome"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            disabled={isLoading || role?.isSystemRole}
          />
          <TextField
            fullWidth
            label="Descrição"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            disabled={isLoading}
            multiline
            rows={2}
          />

          <Box>
            <Typography variant="h6" sx={{ mb: 2 }}>
              Permissões
            </Typography>
            {Object.entries(groupedPermissions).map(([module, perms]) => (
              <Accordion key={module}>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Typography sx={{ fontWeight: 'bold' }}>{module}</Typography>
                  <Chip
                    label={perms.filter(p => selectedPermissions.includes(p.id)).length}
                    size="small"
                    sx={{ ml: 2 }}
                  />
                </AccordionSummary>
                <AccordionDetails>
                  <FormGroup>
                    {perms.map((permission) => (
                      <FormControlLabel
                        key={permission.id}
                        control={
                          <Checkbox
                            checked={selectedPermissions.includes(permission.id)}
                            onChange={() => handlePermissionToggle(permission.id)}
                            disabled={isLoading || role?.isSystemRole}
                          />
                        }
                        label={
                          <Box>
                            <Typography variant="body2" sx={{ fontWeight: 500 }}>
                              {permission.name}
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                              {permission.description}
                            </Typography>
                          </Box>
                        }
                      />
                    ))}
                  </FormGroup>
                </AccordionDetails>
              </Accordion>
            ))}
          </Box>
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={onClose} disabled={isLoading}>
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isLoading || !formData.name || role?.isSystemRole}
        >
          {isLoading ? <CircularProgress size={24} /> : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

// ============================================================================
// Página Principal
// ============================================================================

export const PermissionsPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [tabValue, setTabValue] = useState(0);
  const [roleModalOpen, setRoleModalOpen] = useState(false);
  const [selectedRole, setSelectedRole] = useState<Role | undefined>();

  // Queries
  const { data: roles = [], isLoading: rolesLoading } = useQuery({
    queryKey: ['roles'],
    queryFn: () => roleService.getAll(),
  });

  const { data: permissions = [], isLoading: permissionsLoading } = useQuery({
    queryKey: ['permissions'],
    queryFn: () => permissionService.getAll(),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => roleService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      toast.success('Role removido com sucesso!');
    },
    onError: () => {
      toast.error('Erro ao remover role');
    },
  });

  const handleOpenRoleModal = (role?: Role) => {
    setSelectedRole(role);
    setRoleModalOpen(true);
  };

  const handleCloseRoleModal = () => {
    setRoleModalOpen(false);
    setSelectedRole(undefined);
  };

  const handleDeleteRole = (role: Role) => {
    if (role.isSystemRole) {
      toast.error('Não é possível deletar roles do sistema');
      return;
    }
    if (role.userCount > 0) {
      toast.error('Não é possível deletar role com usuários atribuídos');
      return;
    }
    if (window.confirm(`Tem certeza que deseja deletar o role "${role.name}"?`)) {
      deleteMutation.mutate(role.id);
    }
  };

  const groupedPermissions = permissions.reduce((acc, permission) => {
    if (!acc[permission.module]) {
      acc[permission.module] = [];
    }
    acc[permission.module].push(permission);
    return acc;
  }, {} as Record<string, Permission[]>);

  return (
    <Container maxWidth="lg">
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 1 }}>
          🔐 Gerenciamento de Permissões
        </Typography>
        <Typography variant="body1" color="textSecondary">
          Gerencie roles e permissões do sistema
        </Typography>
      </Box>

      {/* Tabs */}
      <Paper sx={{ mb: 3 }}>
        <Tabs value={tabValue} onChange={(_, newValue) => setTabValue(newValue)}>
          <Tab icon={<GroupIcon />} label="Roles" iconPosition="start" />
          <Tab icon={<SecurityIcon />} label="Permissões" iconPosition="start" />
        </Tabs>
      </Paper>

      {/* Tab: Roles */}
      <TabPanel value={tabValue} index={0}>
        <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">Roles do Sistema</Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => handleOpenRoleModal()}
          >
            Novo Role
          </Button>
        </Box>

        {rolesLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <Grid container spacing={3}>
            {roles.map((role) => (
              <Grid item xs={12} md={6} key={role.id}>
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                      <Box>
                        <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                          {role.name}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          {role.description}
                        </Typography>
                      </Box>
                      <Box>
                        {!role.isSystemRole && (
                          <>
                            <IconButton
                              size="small"
                              onClick={() => handleOpenRoleModal(role)}
                              color="primary"
                            >
                              <EditIcon />
                            </IconButton>
                            <IconButton
                              size="small"
                              onClick={() => handleDeleteRole(role)}
                              color="error"
                              disabled={role.userCount > 0}
                            >
                              <DeleteIcon />
                            </IconButton>
                          </>
                        )}
                      </Box>
                    </Box>

                    <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
                      {role.isSystemRole && (
                        <Chip label="Sistema" size="small" color="primary" variant="outlined" />
                      )}
                      <Chip
                        label={`${role.userCount} usuário${role.userCount !== 1 ? 's' : ''}`}
                        size="small"
                      />
                      <Chip
                        label={`${role.permissions.length} permissõe${role.permissions.length !== 1 ? 's' : ''}`}
                        size="small"
                      />
                    </Box>

                    {role.permissions.length > 0 && (
                      <Box>
                        <Typography variant="caption" color="text.secondary" sx={{ mb: 1, display: 'block' }}>
                          Permissões:
                        </Typography>
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                          {role.permissions.slice(0, 5).map((permission) => (
                            <Chip
                              key={permission.id}
                              label={permission.name}
                              size="small"
                              variant="outlined"
                            />
                          ))}
                          {role.permissions.length > 5 && (
                            <Chip
                              label={`+${role.permissions.length - 5}`}
                              size="small"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </Box>
                    )}
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        )}
      </TabPanel>

      {/* Tab: Permissions */}
      <TabPanel value={tabValue} index={1}>
        {permissionsLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <Box>
            {Object.entries(groupedPermissions).map(([module, perms]) => (
              <Accordion key={module} defaultExpanded>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                    {module}
                  </Typography>
                  <Chip label={perms.length} size="small" sx={{ ml: 2 }} />
                </AccordionSummary>
                <AccordionDetails>
                  <TableContainer>
                    <Table>
                      <TableHead>
                        <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                          <TableCell sx={{ fontWeight: 'bold' }}>Nome</TableCell>
                          <TableCell sx={{ fontWeight: 'bold' }}>Recurso</TableCell>
                          <TableCell sx={{ fontWeight: 'bold' }}>Ação</TableCell>
                          <TableCell sx={{ fontWeight: 'bold' }}>Descrição</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {perms.map((permission) => (
                          <TableRow key={permission.id} hover>
                            <TableCell>{permission.name}</TableCell>
                            <TableCell>
                              <Chip label={permission.resource} size="small" variant="outlined" />
                            </TableCell>
                            <TableCell>
                              <Chip label={permission.action} size="small" />
                            </TableCell>
                            <TableCell>
                              <Typography variant="body2" color="text.secondary">
                                {permission.description}
                              </Typography>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </AccordionDetails>
              </Accordion>
            ))}
          </Box>
        )}
      </TabPanel>

      {/* Role Modal */}
      <RoleModal
        open={roleModalOpen}
        onClose={handleCloseRoleModal}
        role={selectedRole}
        permissions={permissions}
      />
    </Container>
  );
};

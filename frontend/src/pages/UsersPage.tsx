import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Box,
  Button,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  Chip,
  Typography,
  CircularProgress,
  Alert,
  InputAdornment,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  Search as SearchIcon,
  Restore as RestoreIcon,
  Security as SecurityIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userService } from '../services/api/userApi';
import { UserResponse, CreateUserRequest, UpdateUserRequest } from '../types/api';
import { toast } from 'react-toastify';
import { UserRolesModal } from '../components/Users/UserRolesModal';

// ============================================================================
// Modal de Usuário
// ============================================================================

interface UserFormData {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
}

interface UserModalProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: UserFormData) => Promise<void>;
  isLoading: boolean;
  initialData?: UserResponse;
}

const UserModal: React.FC<UserModalProps> = ({
  open,
  onClose,
  onSubmit,
  isLoading,
  initialData,
}) => {
  const [formData, setFormData] = useState<UserFormData>({
    username: '',
    email: '',
    firstName: '',
    lastName: '',
    phone: '',
  });

  useEffect(() => {
    if (initialData) {
      setFormData({
        username: initialData.username,
        email: initialData.email,
        firstName: initialData.firstName,
        lastName: initialData.lastName,
        phone: initialData.phone || '',
      });
    } else {
      setFormData({
        username: '',
        email: '',
        firstName: '',
        lastName: '',
        phone: '',
      });
    }
  }, [initialData, open]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async () => {
    try {
      await onSubmit(formData);
      onClose();
    } catch (error) {
      console.error('Erro ao salvar usuário:', error);
    }
  };

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="sm" 
      fullWidth
      PaperProps={{
        sx: {
          minHeight: '620px',
          maxHeight: '90vh',
        }
      }}
    >
      <DialogTitle sx={{ pb: 1.5, pt: 2.5, fontSize: '1.5rem', fontWeight: 600 }}>
        {initialData ? 'Editar Usuário' : 'Novo Usuário'}
      </DialogTitle>
      <DialogContent sx={{ pt: 3, pb: 2, display: 'flex', flexDirection: 'column', gap: 3 }}>
        <TextField
          fullWidth
          label="Usuário"
          name="username"
          value={formData.username}
          onChange={handleChange}
          disabled={isLoading || !!initialData}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
          sx={{ mt: 1 }}
        />
        <TextField
          fullWidth
          label="Email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          disabled={isLoading || !!initialData}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
        <TextField
          fullWidth
          label="Primeiro Nome"
          name="firstName"
          value={formData.firstName}
          onChange={handleChange}
          disabled={isLoading}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
        <TextField
          fullWidth
          label="Último Nome"
          name="lastName"
          value={formData.lastName}
          onChange={handleChange}
          disabled={isLoading}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
        <TextField
          fullWidth
          label="Telefone"
          name="phone"
          value={formData.phone}
          onChange={handleChange}
          disabled={isLoading}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3, pt: 2 }}>
        <Button onClick={onClose} disabled={isLoading} size="large">
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={isLoading || !formData.email || !formData.firstName}
          size="large"
        >
          {isLoading ? <CircularProgress size={24} /> : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

// ============================================================================
// Página de Gestão de Usuários
// ============================================================================

export const UsersPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [searchTerm, setSearchTerm] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<UserResponse | undefined>();
  const [showDeleted, setShowDeleted] = useState(false);
  const [rolesModalOpen, setRolesModalOpen] = useState(false);
  const [userForRoles, setUserForRoles] = useState<{ id: string; name: string } | null>(null);

  // Queries
  const {
    data: users = [],
    isLoading,
    error,
  } = useQuery({
    queryKey: ['users'],
    queryFn: () => userService.getAll(),
    enabled: !showDeleted,
  });

  const {
    data: deletedUsers = [],
    isLoading: isLoadingDeleted,
    error: errorDeleted,
  } = useQuery({
    queryKey: ['users', 'deleted'],
    queryFn: () => userService.getDeleted(),
    enabled: showDeleted,
  });

  const displayUsers = showDeleted ? deletedUsers : users;
  const displayLoading = showDeleted ? isLoadingDeleted : isLoading;
  const displayError = showDeleted ? errorDeleted : error;

  // Mutations
  const createMutation = useMutation({
    mutationFn: async (data: UserFormData) => {
      const request: CreateUserRequest = {
        username: data.username,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        password: 'TempPassword@123', // TODO: Implementar geração de senha segura
        phone: data.phone,
      };
      return userService.create(request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('Usuário criado com sucesso!');
      setModalOpen(false);
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao criar usuário';
      toast.error(message);
    },
  });

  const updateMutation = useMutation({
    mutationFn: async (data: UserFormData) => {
      if (!selectedUser) throw new Error('Usuário não selecionado');
      const request: UpdateUserRequest = {
        firstName: data.firstName,
        lastName: data.lastName,
        phone: data.phone,
      };
      return userService.update(selectedUser.id, request);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('Usuário atualizado com sucesso!');
      setModalOpen(false);
      setSelectedUser(undefined);
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao atualizar usuário';
      toast.error(message);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => userService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('Usuário removido com sucesso!');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao remover usuário';
      toast.error(message);
    },
  });

  const restoreMutation = useMutation({
    mutationFn: (id: string) => userService.restore(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['users', 'deleted'] });
      toast.success('Usuário restaurado com sucesso!');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao restaurar usuário';
      toast.error(message);
    },
  });

  const handleOpenModal = (user?: UserResponse) => {
    setSelectedUser(user);
    setModalOpen(true);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedUser(undefined);
  };

  const handleSubmit = async (data: UserFormData) => {
    if (selectedUser) {
      await updateMutation.mutateAsync(data);
    } else {
      await createMutation.mutateAsync(data);
    }
  };

  const handleOpenRolesModal = (user: UserResponse) => {
    setUserForRoles({
      id: user.id,
      name: `${user.firstName} ${user.lastName}`,
    });
    setRolesModalOpen(true);
  };

  const handleCloseRolesModal = () => {
    setRolesModalOpen(false);
    setUserForRoles(null);
  };

  const filteredUsers = displayUsers.filter((user) =>
    user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.firstName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <Container maxWidth="lg">
      {/* Header */}
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
          👥 Cadastro de Usuários
        </Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button
            variant={showDeleted ? 'contained' : 'outlined'}
            onClick={() => setShowDeleted(!showDeleted)}
            startIcon={<RestoreIcon />}
          >
            {showDeleted ? 'Ver Ativos' : 'Ver Deletados'}
          </Button>
          {!showDeleted && (
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenModal()}
            >
              Novo Usuário
            </Button>
          )}
        </Box>
      </Box>

      {/* Search */}
      <Paper sx={{ mb: 3, p: 2 }}>
        <TextField
          fullWidth
          placeholder="Buscar usuário por nome, email ou usuário..."
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
      </Paper>

      {/* Error Alert */}
      {displayError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Erro ao carregar usuários. Tente novamente.
        </Alert>
      )}

      {/* Loading */}
      {displayLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
          <CircularProgress />
        </Box>
      )}

      {/* Users Table */}
      {!displayLoading && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                <TableCell sx={{ fontWeight: 'bold' }}>Usuário</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Email</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Nome</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Status</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }} align="right">
                  Ações
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredUsers.length > 0 ? (
                filteredUsers.map((user) => (
                  <TableRow key={user.id} hover>
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                        {user.username}
                      </Typography>
                    </TableCell>
                    <TableCell>{user.email}</TableCell>
                    <TableCell>
                      {user.firstName} {user.lastName}
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={user.isActive ? 'Ativo' : 'Inativo'}
                        color={user.isActive ? 'success' : 'error'}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="right">
                      {showDeleted ? (
                        <IconButton
                          size="small"
                          onClick={() => restoreMutation.mutate(user.id)}
                          color="success"
                          disabled={restoreMutation.isPending}
                          title="Restaurar usuário"
                        >
                          <RestoreIcon />
                        </IconButton>
                      ) : (
                        <>
                          <IconButton
                            size="small"
                            onClick={() => handleOpenRolesModal(user)}
                            color="secondary"
                            title="Gerenciar roles"
                          >
                            <SecurityIcon />
                          </IconButton>
                          <IconButton
                            size="small"
                            onClick={() => handleOpenModal(user)}
                            color="primary"
                            title="Editar usuário"
                          >
                            <EditIcon />
                          </IconButton>
                          <IconButton
                            size="small"
                            onClick={() => deleteMutation.mutate(user.id)}
                            color="error"
                            disabled={deleteMutation.isPending}
                            title="Deletar usuário"
                          >
                            <DeleteIcon />
                          </IconButton>
                        </>
                      )}
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={5} sx={{ textAlign: 'center', py: 4 }}>
                    <Typography color="textSecondary">
                      {searchTerm 
                        ? 'Nenhum usuário encontrado' 
                        : showDeleted 
                          ? 'Nenhum usuário deletado' 
                          : 'Nenhum usuário cadastrado'}
                    </Typography>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Modal */}
      <UserModal
        open={modalOpen}
        onClose={handleCloseModal}
        onSubmit={handleSubmit}
        isLoading={createMutation.isPending || updateMutation.isPending}
        initialData={selectedUser}
      />

      {/* Modal de Gerenciamento de Roles */}
      <UserRolesModal
        open={rolesModalOpen}
        onClose={handleCloseRolesModal}
        userId={userForRoles?.id || ''}
        userName={userForRoles?.name || ''}
      />
    </Container>
  );
};

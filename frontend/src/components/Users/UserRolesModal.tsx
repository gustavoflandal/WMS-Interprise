import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  Checkbox,
  FormControlLabel,
  FormGroup,
  CircularProgress,
  Alert,
  IconButton,
  Chip,
} from '@mui/material';
import { Close } from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { roleService } from '../../services/api/roleApi';
import { userService } from '../../services/api/userApi';
import { toast } from 'react-toastify';

interface UserRolesModalProps {
  open: boolean;
  onClose: () => void;
  userId: string;
  userName: string;
}

export const UserRolesModal: React.FC<UserRolesModalProps> = ({
  open,
  onClose,
  userId,
  userName,
}) => {
  const queryClient = useQueryClient();
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);

  // Buscar todos os roles disponíveis
  const { data: availableRoles = [], isLoading: rolesLoading } = useQuery({
    queryKey: ['roles'],
    queryFn: () => roleService.getAll(),
    enabled: open,
  });

  // Buscar roles atuais do usuário
  const { data: userRoles = [], isLoading: userRolesLoading } = useQuery({
    queryKey: ['user-roles', userId],
    queryFn: () => userService.getUserRoles(userId),
    enabled: open && !!userId,
  });

  useEffect(() => {
    if (!open) {
      setSelectedRoles([]);
      return;
    }
    
    if (userRoles && userRoles.length > 0) {
      const roleIds = userRoles.map((ur: any) => ur.id);
      setSelectedRoles(roleIds);
    } else if (userRoles && userRoles.length === 0) {
      setSelectedRoles([]);
    }
  }, [open, userId]); // Removido userRoles da dependência

  // Atualizar selectedRoles quando userRoles mudar, mas apenas se já tiver carregado
  useEffect(() => {
    if (!userRolesLoading && userRoles) {
      const roleIds = userRoles.map((ur: any) => ur.id);
      const currentIds = JSON.stringify(selectedRoles.sort());
      const newIds = JSON.stringify(roleIds.sort());
      
      if (currentIds !== newIds) {
        setSelectedRoles(roleIds);
      }
    }
  }, [userRolesLoading]); // Só executa quando userRolesLoading muda

  const assignRolesMutation = useMutation({
    mutationFn: (roleIds: string[]) => userService.assignRoles(userId, roleIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user-roles', userId] });
      toast.success('Roles atribuídos com sucesso!');
      onClose();
    },
    onError: () => {
      toast.error('Erro ao atribuir roles');
    },
  });

  const handleRoleToggle = (roleId: string) => {
    setSelectedRoles(prev =>
      prev.includes(roleId)
        ? prev.filter(id => id !== roleId)
        : [...prev, roleId]
    );
  };

  const handleSubmit = () => {
    assignRolesMutation.mutate(selectedRoles);
  };

  const isLoading = rolesLoading || userRolesLoading || assignRolesMutation.isPending;

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center',
        pb: 1,
      }}>
        <Box>
          <Typography variant="h5" component="div" sx={{ fontWeight: 'bold' }}>
            Gerenciar Roles
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Usuário: {userName}
          </Typography>
        </Box>
        <IconButton
          onClick={onClose}
          disabled={isLoading}
          size="small"
          sx={{ color: 'text.secondary' }}
        >
          <Close />
        </IconButton>
      </DialogTitle>

      <DialogContent dividers>
        {rolesLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : availableRoles.length === 0 ? (
          <Alert severity="info">
            Nenhum role disponível no sistema.
          </Alert>
        ) : (
          <FormGroup>
            {availableRoles.map((role) => (
              <Box
                key={role.id}
                sx={{
                  p: 2,
                  mb: 1,
                  border: '1px solid',
                  borderColor: selectedRoles.includes(role.id) ? 'primary.main' : 'divider',
                  borderRadius: 1,
                  backgroundColor: selectedRoles.includes(role.id) ? 'primary.light' : 'transparent',
                  transition: 'all 0.2s',
                  '&:hover': {
                    borderColor: 'primary.main',
                    backgroundColor: 'action.hover',
                  },
                }}
              >
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={selectedRoles.includes(role.id)}
                      onChange={() => handleRoleToggle(role.id)}
                      disabled={isLoading}
                    />
                  }
                  label={
                    <Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
                        <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
                          {role.name}
                        </Typography>
                        {role.isSystemRole && (
                          <Chip label="Sistema" size="small" color="primary" variant="outlined" />
                        )}
                      </Box>
                      <Typography variant="body2" color="text.secondary">
                        {role.description}
                      </Typography>
                      <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
                        {role.permissions.length} permissõe{role.permissions.length !== 1 ? 's' : ''}
                      </Typography>
                    </Box>
                  }
                  sx={{ width: '100%', m: 0 }}
                />
              </Box>
            ))}
          </FormGroup>
        )}

        <Box sx={{ mt: 2 }}>
          <Typography variant="caption" color="text.secondary">
            {selectedRoles.length} role{selectedRoles.length !== 1 ? 's' : ''} selecionado{selectedRoles.length !== 1 ? 's' : ''}
          </Typography>
        </Box>
      </DialogContent>

      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button 
          onClick={onClose} 
          disabled={isLoading}
          variant="outlined"
        >
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          disabled={isLoading}
          variant="contained"
          sx={{ minWidth: 120 }}
        >
          {isLoading ? <CircularProgress size={24} /> : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

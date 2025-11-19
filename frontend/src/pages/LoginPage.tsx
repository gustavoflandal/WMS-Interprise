import React, { useState } from 'react';
import {
  Container,
  Paper,
  TextField,
  Button,
  Box,
  Typography,
  Link,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { ForgotPasswordModal } from '../components/Auth';
import { userService } from '../services/api/userApi';
import { useMutation } from '@tanstack/react-query';
import { toast } from 'react-toastify';

// ============================================================================
// Modal de Novo Usuário (Cadastro)
// ============================================================================

interface UserFormData {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone?: string;
}

interface UserModalProps {
  open: boolean;
  onClose: () => void;
}

const UserModal: React.FC<UserModalProps> = ({ open, onClose }) => {
  const [formData, setFormData] = useState<UserFormData>({
    username: '',
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    phone: '',
  });

  const [showPassword, setShowPassword] = useState(false);

  const createMutation = useMutation({
    mutationFn: (data: UserFormData) => userService.create(data),
    onSuccess: () => {
      toast.success('Usuário criado com sucesso! Faça login para acessar o sistema.');
      handleClose();
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Erro ao criar usuário');
    },
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async () => {
    if (!formData.username || !formData.email || !formData.password || !formData.firstName) {
      toast.error('Preencha todos os campos obrigatórios');
      return;
    }
    if (formData.password.length < 6) {
      toast.error('A senha deve ter no mínimo 6 caracteres');
      return;
    }
    createMutation.mutate(formData);
  };

  const handleClose = () => {
    setFormData({
      username: '',
      email: '',
      password: '',
      firstName: '',
      lastName: '',
      phone: '',
    });
    setShowPassword(false);
    onClose();
  };

  return (
    <Dialog 
      open={open} 
      onClose={handleClose} 
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
        Novo Usuário
      </DialogTitle>
      <DialogContent sx={{ pt: 3, pb: 2, display: 'flex', flexDirection: 'column', gap: 3 }}>
        <TextField
          fullWidth
          required
          label="Usuário"
          name="username"
          value={formData.username}
          onChange={handleChange}
          disabled={createMutation.isPending}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
          sx={{ mt: 1 }}
        />
        <TextField
          fullWidth
          required
          label="Email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          disabled={createMutation.isPending}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
        <TextField
          fullWidth
          required
          label="Senha"
          name="password"
          type={showPassword ? 'text' : 'password'}
          value={formData.password}
          onChange={handleChange}
          disabled={createMutation.isPending}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  onClick={() => setShowPassword(!showPassword)}
                  edge="end"
                >
                  {showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            ),
          }}
          helperText="Mínimo 6 caracteres"
        />
        <TextField
          fullWidth
          required
          label="Primeiro Nome"
          name="firstName"
          value={formData.firstName}
          onChange={handleChange}
          disabled={createMutation.isPending}
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
          disabled={createMutation.isPending}
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
          disabled={createMutation.isPending}
          InputLabelProps={{
            sx: { fontSize: '0.95rem', fontWeight: 500 },
            shrink: true,
          }}
        />
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3, pt: 2 }}>
        <Button onClick={handleClose} disabled={createMutation.isPending} size="large">
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={createMutation.isPending || !formData.email || !formData.firstName}
          size="large"
        >
          {createMutation.isPending ? <CircularProgress size={24} /> : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

// ============================================================================
// Tela de Login
// ============================================================================

export const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login, isLoading, error } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const [openRegisterModal, setOpenRegisterModal] = useState(false);
  const [openForgotPasswordModal, setOpenForgotPasswordModal] = useState(false);
  const [formData, setFormData] = useState({
    username: '',
    password: '',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.username && formData.password) {
      login(formData.username, formData.password);
    }
  };

  const handleClickShowPassword = () => {
    setShowPassword(!showPassword);
  };

  const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
  };

  return (
    <>
      <Container
        maxWidth="sm"
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          backgroundColor: '#f5f5f5',
        }}
      >
        <Paper
          elevation={6}
          sx={{
            padding: 4,
            display: 'flex',
            flexDirection: 'column',
            gap: 1.63,
            width: '100%',
            borderRadius: 3,
            backgroundColor: '#ffffff',
            boxShadow: '0 0 20px rgba(0, 0, 0, 0.08)',
          }}
        >
          {/* Header com Logo */}
          <Box sx={{ textAlign: 'center', mb: 2 }}>
            <Box
              component="img"
              src="/WMS-Interprise-Logo.png"
              alt="WMS Interprise Logo"
              sx={{
                maxWidth: '400px',
                width: '100%',
                height: 'auto',
                display: 'block',
                margin: '0 auto',
              }}
              onError={(e) => {
                // Fallback caso a imagem não carregue
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
              }}
            />
          </Box>

        {/* Erro */}
        {error && (
          <Alert severity="error">
            {typeof error === 'string' ? error : 'Erro ao fazer login. Tente novamente.'}
          </Alert>
        )}

        {/* Form */}
        <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {/* Username/Email */}
          <TextField
            fullWidth
            label="Usuário ou Email"
            name="username"
            value={formData.username}
            onChange={handleChange}
            placeholder="Digite seu usuário ou email"
            disabled={isLoading}
            autoComplete="username"
            variant="outlined"
          />

          {/* Password */}
          <TextField
            fullWidth
            label="Senha"
            name="password"
            type={showPassword ? 'text' : 'password'}
            value={formData.password}
            onChange={handleChange}
            placeholder="Digite sua senha"
            disabled={isLoading}
            autoComplete="current-password"
            variant="outlined"
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={handleClickShowPassword}
                    onMouseDown={handleMouseDownPassword}
                    edge="end"
                    disabled={isLoading}
                  >
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />

          {/* Submit Button */}
          <Button
            type="submit"
            fullWidth
            variant="contained"
            size="large"
            disabled={isLoading || !formData.username || !formData.password}
            sx={{
              mt: 2,
              textTransform: 'uppercase',
              fontWeight: 'bold',
            }}
          >
            {isLoading ? <CircularProgress size={24} /> : 'Entrar'}
          </Button>
        </Box>

        {/* Divider */}
        <Box sx={{ borderTop: '1px solid #e0e0e0', my: 2 }} />

        {/* Footer Links */}
        <Box sx={{ textAlign: 'center', display: 'flex', flexDirection: 'column', gap: 1 }}>
          <Typography variant="body2">
            Não tem conta?{' '}
            <Link
              component="button"
              variant="body2"
              onClick={() => setOpenRegisterModal(true)}
              sx={{ textDecoration: 'none', cursor: 'pointer', fontWeight: 'bold' }}
            >
              Criar conta
            </Link>
          </Typography>
          <Typography variant="body2">
            <Link
              component="button"
              variant="body2"
              onClick={() => setOpenForgotPasswordModal(true)}
              sx={{ textDecoration: 'none', cursor: 'pointer' }}
            >
              Esqueceu a senha?
            </Link>
          </Typography>
        </Box>

        {/* Demo Credentials */}
        <Box
          sx={{
            backgroundColor: '#f5f5f5',
            padding: 2,
            borderRadius: 1,
            textAlign: 'center',
            mt: 2,
          }}
        >
          <Typography variant="caption" display="block" gutterBottom>
            🔓 Credenciais de Demonstração
          </Typography>
          <Typography variant="caption" display="block">
            <strong>Usuário:</strong> admin
          </Typography>
          <Typography variant="caption" display="block">
            <strong>Senha:</strong> Admin@123
          </Typography>
        </Box>
      </Paper>
    </Container>

    {/* Modais */}
    <UserModal 
      open={openRegisterModal} 
      onClose={() => setOpenRegisterModal(false)} 
    />
    <ForgotPasswordModal 
      open={openForgotPasswordModal} 
      onClose={() => setOpenForgotPasswordModal(false)} 
    />
    </>
  );
};

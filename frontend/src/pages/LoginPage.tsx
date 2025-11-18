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
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { RegisterModal, ForgotPasswordModal } from '../components/Auth';

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
    <RegisterModal 
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

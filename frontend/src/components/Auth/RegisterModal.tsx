import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  Typography,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
  Grid,
} from '@mui/material';
import { Visibility, VisibilityOff, Close } from '@mui/icons-material';

interface RegisterModalProps {
  open: boolean;
  onClose: () => void;
}

export const RegisterModal: React.FC<RegisterModalProps> = ({ open, onClose }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    username: '',
    password: '',
    confirmPassword: '',
    company: '',
    phone: '',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    // Limpa erro ao digitar
    if (error) setError(null);
  };

  const validateForm = (): boolean => {
    if (!formData.fullName || !formData.email || !formData.username || !formData.password) {
      setError('Por favor, preencha todos os campos obrigatórios.');
      return false;
    }

    if (formData.password.length < 8) {
      setError('A senha deve ter pelo menos 8 caracteres.');
      return false;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('As senhas não coincidem.');
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.email)) {
      setError('Por favor, insira um email válido.');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      // TODO: Implementar chamada à API de registro
      // const response = await authService.register(formData);
      
      // Simulação de chamada à API
      await new Promise((resolve) => setTimeout(resolve, 1500));

      setSuccess(true);
      
      // Fechar modal após 2 segundos e resetar formulário
      setTimeout(() => {
        handleClose();
      }, 2000);
    } catch (err: any) {
      setError(err.message || 'Erro ao criar conta. Tente novamente.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    if (!isLoading) {
      setFormData({
        fullName: '',
        email: '',
        username: '',
        password: '',
        confirmPassword: '',
        company: '',
        phone: '',
      });
      setError(null);
      setSuccess(false);
      setShowPassword(false);
      setShowConfirmPassword(false);
      onClose();
    }
  };

  return (
    <Dialog 
      open={open} 
      onClose={handleClose}
      maxWidth="sm"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 2,
        }
      }}
    >
      <DialogTitle sx={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center',
        pb: 1,
      }}>
        <Typography variant="h5" component="div" sx={{ fontWeight: 'bold' }}>
          Criar Nova Conta
        </Typography>
        <IconButton
          onClick={handleClose}
          disabled={isLoading}
          size="small"
          sx={{ color: 'text.secondary' }}
        >
          <Close />
        </IconButton>
      </DialogTitle>

      <DialogContent dividers>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            Conta criada com sucesso! Você já pode fazer login.
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
          <Grid container spacing={2}>
            {/* Nome Completo */}
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Nome Completo"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="name"
                placeholder="Digite seu nome completo"
              />
            </Grid>

            {/* Email */}
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="email"
                placeholder="seu.email@exemplo.com"
              />
            </Grid>

            {/* Username */}
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Nome de Usuário"
                name="username"
                value={formData.username}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="username"
                placeholder="Digite seu nome de usuário"
              />
            </Grid>

            {/* Empresa */}
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Empresa"
                name="company"
                value={formData.company}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="organization"
                placeholder="Nome da empresa (opcional)"
              />
            </Grid>

            {/* Telefone */}
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Telefone"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="tel"
                placeholder="(00) 00000-0000"
              />
            </Grid>

            {/* Senha */}
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Senha"
                name="password"
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="new-password"
                placeholder="Mínimo 8 caracteres"
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        onClick={() => setShowPassword(!showPassword)}
                        edge="end"
                        disabled={isLoading || success}
                      >
                        {showPassword ? <VisibilityOff /> : <Visibility />}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
              />
            </Grid>

            {/* Confirmar Senha */}
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Confirmar Senha"
                name="confirmPassword"
                type={showConfirmPassword ? 'text' : 'password'}
                value={formData.confirmPassword}
                onChange={handleChange}
                disabled={isLoading || success}
                autoComplete="new-password"
                placeholder="Digite a senha novamente"
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        edge="end"
                        disabled={isLoading || success}
                      >
                        {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
              />
            </Grid>
          </Grid>

          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 2 }}>
            * Campos obrigatórios
          </Typography>
        </Box>
      </DialogContent>

      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button 
          onClick={handleClose} 
          disabled={isLoading}
          variant="outlined"
        >
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          disabled={isLoading || success}
          variant="contained"
          sx={{ minWidth: 120 }}
        >
          {isLoading ? <CircularProgress size={24} /> : 'Criar Conta'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

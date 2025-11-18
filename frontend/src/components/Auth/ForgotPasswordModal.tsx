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
  IconButton,
} from '@mui/material';
import { Close, EmailOutlined } from '@mui/icons-material';

interface ForgotPasswordModalProps {
  open: boolean;
  onClose: () => void;
}

export const ForgotPasswordModal: React.FC<ForgotPasswordModalProps> = ({ open, onClose }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [email, setEmail] = useState('');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
    // Limpa erro ao digitar
    if (error) setError(null);
  };

  const validateEmail = (): boolean => {
    if (!email) {
      setError('Por favor, insira seu email.');
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setError('Por favor, insira um email válido.');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!validateEmail()) {
      return;
    }

    setIsLoading(true);

    try {
      // TODO: Implementar chamada à API de recuperação de senha
      // const response = await authService.forgotPassword(email);
      
      // Simulação de chamada à API
      await new Promise((resolve) => setTimeout(resolve, 1500));

      setSuccess(true);
      
      // Fechar modal após 3 segundos
      setTimeout(() => {
        handleClose();
      }, 3000);
    } catch (err: any) {
      setError(err.message || 'Erro ao enviar email. Tente novamente.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    if (!isLoading) {
      setEmail('');
      setError(null);
      setSuccess(false);
      onClose();
    }
  };

  return (
    <Dialog 
      open={open} 
      onClose={handleClose}
      maxWidth="xs"
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
          Recuperar Senha
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
        {!success ? (
          <>
            <Box sx={{ textAlign: 'center', mb: 3 }}>
              <EmailOutlined sx={{ fontSize: 60, color: 'primary.main', mb: 2 }} />
              <Typography variant="body2" color="text.secondary">
                Digite seu email cadastrado e enviaremos um link para redefinir sua senha.
              </Typography>
            </Box>

            {error && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit}>
              <TextField
                fullWidth
                required
                label="Email"
                name="email"
                type="email"
                value={email}
                onChange={handleChange}
                disabled={isLoading}
                autoComplete="email"
                placeholder="seu.email@exemplo.com"
                autoFocus
                InputProps={{
                  startAdornment: (
                    <Box sx={{ mr: 1, display: 'flex', alignItems: 'center' }}>
                      <EmailOutlined color="action" />
                    </Box>
                  ),
                }}
              />
            </Box>
          </>
        ) : (
          <Alert severity="success" icon={<EmailOutlined />}>
            <Typography variant="body2" sx={{ fontWeight: 'bold', mb: 1 }}>
              Email enviado com sucesso!
            </Typography>
            <Typography variant="body2">
              Verifique sua caixa de entrada e siga as instruções para redefinir sua senha.
            </Typography>
            <Typography variant="caption" display="block" sx={{ mt: 1, color: 'text.secondary' }}>
              Não esqueça de verificar a pasta de spam.
            </Typography>
          </Alert>
        )}
      </DialogContent>

      <DialogActions sx={{ px: 3, py: 2 }}>
        {!success ? (
          <>
            <Button 
              onClick={handleClose} 
              disabled={isLoading}
              variant="outlined"
            >
              Cancelar
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={isLoading || !email}
              variant="contained"
              sx={{ minWidth: 120 }}
            >
              {isLoading ? <CircularProgress size={24} /> : 'Enviar Email'}
            </Button>
          </>
        ) : (
          <Button 
            onClick={handleClose} 
            variant="contained"
            fullWidth
          >
            Fechar
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};

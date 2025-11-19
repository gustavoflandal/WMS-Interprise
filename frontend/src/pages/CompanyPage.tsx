import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Box,
  Typography,
  TextField,
  Button,
  Grid,
  Divider,
  CircularProgress,
  Alert,
  InputAdornment,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import {
  Business as BusinessIcon,
  Save as SaveIcon,
  Phone as PhoneIcon,
  Email as EmailIcon,
  LocationOn as LocationIcon,
  Description as DescriptionIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import { companyService } from '../services/api/companyApi';
import { brazilianStates } from '../utils/brazilianStates';
import { formatCNPJ, formatPhone, formatCEP } from '../utils/formatters';

// ============================================================================
// Página de Cadastro da Empresa
// ============================================================================

interface CompanyFormData {
  // Dados Principais
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  inscricaoEstadual: string;
  inscricaoMunicipal: string;
  
  // Contato
  email: string;
  telefone: string;
  celular: string;
  website: string;
  
  // Endereço
  cep: string;
  logradouro: string;
  numero: string;
  complemento: string;
  bairro: string;
  cidade: string;
  estado: string;
  
  // Informações Adicionais
  dataAbertura: string;
  capitalSocial: string;
  atividadePrincipal: string;
  regimeTributario: string;
  
  // Responsável Legal
  nomeResponsavel: string;
  cpfResponsavel: string;
  emailResponsavel: string;
  telefoneResponsavel: string;
  cargoResponsavel: string;
}

export const CompanyPage: React.FC = () => {
  const queryClient = useQueryClient();
  
  const [formData, setFormData] = useState<CompanyFormData>({
    razaoSocial: '',
    nomeFantasia: '',
    cnpj: '',
    inscricaoEstadual: '',
    inscricaoMunicipal: '',
    email: '',
    telefone: '',
    celular: '',
    website: '',
    cep: '',
    logradouro: '',
    numero: '',
    complemento: '',
    bairro: '',
    cidade: '',
    estado: '',
    dataAbertura: '',
    capitalSocial: '',
    atividadePrincipal: '',
    regimeTributario: 'SIMPLES_NACIONAL',
    nomeResponsavel: '',
    cpfResponsavel: '',
    emailResponsavel: '',
    telefoneResponsavel: '',
    cargoResponsavel: '',
  });

  // Buscar dados da empresa
  const { data: company, isLoading } = useQuery({
    queryKey: ['company'],
    queryFn: () => companyService.get(),
  });

  // Sincronizar formData quando os dados da empresa forem carregados
  useEffect(() => {
    if (company) {
      setFormData({
        razaoSocial: company.razaoSocial || '',
        nomeFantasia: company.nomeFantasia || '',
        cnpj: company.cnpj || '',
        inscricaoEstadual: company.inscricaoEstadual || '',
        inscricaoMunicipal: company.inscricaoMunicipal || '',
        email: company.email || '',
        telefone: company.telefone || '',
        celular: company.celular || '',
        website: company.website || '',
        cep: company.cep || '',
        logradouro: company.logradouro || '',
        numero: company.numero || '',
        complemento: company.complemento || '',
        bairro: company.bairro || '',
        cidade: company.cidade || '',
        estado: company.estado || '',
        dataAbertura: company.dataAbertura ? company.dataAbertura.split('T')[0] : '',
        capitalSocial: company.capitalSocial?.toString() || '',
        atividadePrincipal: company.atividadePrincipal || '',
        regimeTributario: company.regimeTributario || 'SIMPLES_NACIONAL',
        nomeResponsavel: company.nomeResponsavel || '',
        cpfResponsavel: company.cpfResponsavel || '',
        emailResponsavel: company.emailResponsavel || '',
        telefoneResponsavel: company.telefoneResponsavel || '',
        cargoResponsavel: company.cargoResponsavel || '',
      });
    }
  }, [company]);

  // Mutation para salvar/atualizar empresa
  const saveMutation = useMutation({
    mutationFn: (data: CompanyFormData) => {
      if (company) {
        return companyService.update(data);
      }
      return companyService.create(data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['company'] });
      toast.success(company ? 'Empresa atualizada com sucesso!' : 'Empresa cadastrada com sucesso!');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao salvar empresa';
      toast.error(message);
    },
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSelectChange = (name: string, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validações
    if (!formData.razaoSocial || !formData.cnpj || !formData.email) {
      toast.error('Preencha todos os campos obrigatórios');
      return;
    }

    saveMutation.mutate(formData);
  };

  const handleCEPSearch = async () => {
    const cep = formData.cep.replace(/\D/g, '');
    
    if (cep.length !== 8) {
      toast.error('CEP inválido');
      return;
    }

    try {
      const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
      const data = await response.json();
      
      if (data.erro) {
        toast.error('CEP não encontrado');
        return;
      }

      setFormData((prev) => ({
        ...prev,
        logradouro: data.logradouro || '',
        bairro: data.bairro || '',
        cidade: data.localidade || '',
        estado: data.uf || '',
      }));

      toast.success('Endereço encontrado!');
    } catch (error) {
      toast.error('Erro ao buscar CEP');
    }
  };

  if (isLoading) {
    return (
      <Container maxWidth="lg">
        <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg">
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
          <BusinessIcon fontSize="large" />
          Cadastro da Empresa
        </Typography>
        <Typography variant="body1" color="textSecondary">
          {company ? 'Atualize as informações da sua empresa' : 'Cadastre as informações da sua empresa'}
        </Typography>
      </Box>

      <form onSubmit={handleSubmit}>
        <Paper sx={{ p: 4, mb: 3 }}>
          {/* Seção: Dados Principais */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: 1 }}>
              <BusinessIcon />
              Dados Principais
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={8}>
                <TextField
                  fullWidth
                  required
                  label="Razão Social"
                  name="razaoSocial"
                  value={formData.razaoSocial}
                  onChange={handleChange}
                  placeholder="Nome completo da empresa"
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  required
                  label="CNPJ"
                  name="cnpj"
                  value={formatCNPJ(formData.cnpj)}
                  onChange={handleChange}
                  placeholder="00.000.000/0000-00"
                  inputProps={{ maxLength: 18 }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Nome Fantasia"
                  name="nomeFantasia"
                  value={formData.nomeFantasia}
                  onChange={handleChange}
                  placeholder="Nome comercial"
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Inscrição Estadual"
                  name="inscricaoEstadual"
                  value={formData.inscricaoEstadual}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Inscrição Municipal"
                  name="inscricaoMunicipal"
                  value={formData.inscricaoMunicipal}
                  onChange={handleChange}
                />
              </Grid>
            </Grid>
          </Box>

          <Divider sx={{ my: 4 }} />

          {/* Seção: Contato */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: 1 }}>
              <PhoneIcon />
              Informações de Contato
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="E-mail"
                  name="email"
                  type="email"
                  value={formData.email}
                  onChange={handleChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <EmailIcon />
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Telefone"
                  name="telefone"
                  value={formatPhone(formData.telefone)}
                  onChange={handleChange}
                  placeholder="(00) 0000-0000"
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <PhoneIcon />
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Celular"
                  name="celular"
                  value={formatPhone(formData.celular)}
                  onChange={handleChange}
                  placeholder="(00) 00000-0000"
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <PhoneIcon />
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Website"
                  name="website"
                  value={formData.website}
                  onChange={handleChange}
                  placeholder="www.suaempresa.com.br"
                />
              </Grid>
            </Grid>
          </Box>

          <Divider sx={{ my: 4 }} />

          {/* Seção: Endereço */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: 1 }}>
              <LocationIcon />
              Endereço
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  required
                  label="CEP"
                  name="cep"
                  value={formatCEP(formData.cep)}
                  onChange={handleChange}
                  onBlur={handleCEPSearch}
                  placeholder="00000-000"
                  inputProps={{ maxLength: 9 }}
                />
              </Grid>
              <Grid item xs={12} md={7}>
                <TextField
                  fullWidth
                  required
                  label="Logradouro"
                  name="logradouro"
                  value={formData.logradouro}
                  onChange={handleChange}
                  placeholder="Rua, Avenida, etc."
                />
              </Grid>
              <Grid item xs={12} md={2}>
                <TextField
                  fullWidth
                  required
                  label="Número"
                  name="numero"
                  value={formData.numero}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  label="Complemento"
                  name="complemento"
                  value={formData.complemento}
                  onChange={handleChange}
                  placeholder="Apto, Sala, etc."
                />
              </Grid>
              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  required
                  label="Bairro"
                  name="bairro"
                  value={formData.bairro}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  required
                  label="Cidade"
                  name="cidade"
                  value={formData.cidade}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={1}>
                <FormControl fullWidth required>
                  <InputLabel>UF</InputLabel>
                  <Select
                    value={formData.estado}
                    label="UF"
                    onChange={(e) => handleSelectChange('estado', e.target.value)}
                  >
                    {brazilianStates.map((state) => (
                      <MenuItem key={state.value} value={state.value}>
                        {state.value}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
            </Grid>
          </Box>

          <Divider sx={{ my: 4 }} />

          {/* Seção: Informações Adicionais */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: 1 }}>
              <DescriptionIcon />
              Informações Adicionais
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Data de Abertura"
                  name="dataAbertura"
                  type="date"
                  value={formData.dataAbertura}
                  onChange={handleChange}
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Capital Social"
                  name="capitalSocial"
                  value={formData.capitalSocial}
                  onChange={handleChange}
                  placeholder="R$ 0,00"
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <FormControl fullWidth>
                  <InputLabel>Regime Tributário</InputLabel>
                  <Select
                    value={formData.regimeTributario}
                    label="Regime Tributário"
                    onChange={(e) => handleSelectChange('regimeTributario', e.target.value)}
                  >
                    <MenuItem value="SIMPLES_NACIONAL">Simples Nacional</MenuItem>
                    <MenuItem value="LUCRO_PRESUMIDO">Lucro Presumido</MenuItem>
                    <MenuItem value="LUCRO_REAL">Lucro Real</MenuItem>
                    <MenuItem value="MEI">MEI</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} md={9}>
                <TextField
                  fullWidth
                  label="Atividade Principal (CNAE)"
                  name="atividadePrincipal"
                  value={formData.atividadePrincipal}
                  onChange={handleChange}
                  placeholder="Descrição da atividade principal"
                />
              </Grid>
            </Grid>
          </Box>

          <Divider sx={{ my: 4 }} />

          {/* Seção: Responsável Legal */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold' }}>
              👤 Responsável Legal
            </Typography>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="Nome Completo"
                  name="nomeResponsavel"
                  value={formData.nomeResponsavel}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  required
                  label="CPF"
                  name="cpfResponsavel"
                  value={formData.cpfResponsavel}
                  onChange={handleChange}
                  placeholder="000.000.000-00"
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Cargo"
                  name="cargoResponsavel"
                  value={formData.cargoResponsavel}
                  onChange={handleChange}
                  placeholder="Ex: Diretor, Sócio"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  required
                  label="E-mail"
                  name="emailResponsavel"
                  type="email"
                  value={formData.emailResponsavel}
                  onChange={handleChange}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Telefone"
                  name="telefoneResponsavel"
                  value={formatPhone(formData.telefoneResponsavel)}
                  onChange={handleChange}
                  placeholder="(00) 00000-0000"
                />
              </Grid>
            </Grid>
          </Box>

          {/* Botões de Ação */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2, mt: 4 }}>
            <Button
              variant="outlined"
              size="large"
              onClick={() => window.history.back()}
              disabled={saveMutation.isPending}
            >
              Cancelar
            </Button>
            <Button
              type="submit"
              variant="contained"
              size="large"
              startIcon={saveMutation.isPending ? <CircularProgress size={20} /> : <SaveIcon />}
              disabled={saveMutation.isPending}
            >
              {saveMutation.isPending ? 'Salvando...' : 'Salvar'}
            </Button>
          </Box>
        </Paper>
      </form>

      {/* Informações de Ajuda */}
      <Alert severity="info" sx={{ mb: 3 }}>
        <Typography variant="body2" sx={{ fontWeight: 'bold', mb: 1 }}>
          💡 Dicas:
        </Typography>
        <Typography variant="body2" component="ul" sx={{ pl: 2, m: 0 }}>
          <li>Mantenha os dados da empresa sempre atualizados</li>
          <li>O CNPJ e Razão Social não podem ser alterados após o cadastro</li>
          <li>Digite o CEP e pressione Tab para buscar o endereço automaticamente</li>
          <li>Todos os campos marcados com * são obrigatórios</li>
        </Typography>
      </Alert>
    </Container>
  );
};

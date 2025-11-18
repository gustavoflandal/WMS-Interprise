# WMS Interprise

Sistema de Gerenciamento de Armazém (Warehouse Management System) de alta performance desenvolvido com tecnologias modernas.

## Tecnologias

### Backend
- **.NET 10** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados principal
- **Redis** - Cache distribuído
- **Apache Kafka** - Message broker
- **Elasticsearch** - Busca e logging
- **Serilog** - Logging estruturado
- **xUnit** - Framework de testes

### Frontend
- **React 18** - Biblioteca UI
- **TypeScript** - Linguagem
- **Vite** - Build tool
- **Material-UI (MUI)** - Componentes UI
- **Zustand** - Gerenciamento de estado
- **TanStack Query** - Data fetching
- **Axios** - Cliente HTTP
- **React Router** - Roteamento
- **Vitest** - Framework de testes

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **Kubernetes** - Orquestração em produção
- **GitHub Actions** - CI/CD
- **Terraform** - Infrastructure as Code
- **Prometheus** - Monitoramento
- **Grafana** - Dashboards

## Estrutura do Projeto

```
WMS-Interprise/
├── backend/              # Backend .NET
│   ├── src/
│   │   ├── WMS.API/
│   │   ├── WMS.Domain/
│   │   ├── WMS.Application/
│   │   ├── WMS.Infrastructure/
│   │   └── WMS.Shared/
│   └── tests/
│
├── frontend/             # Frontend React
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── hooks/
│   │   ├── services/
│   │   └── store/
│   └── tests/
│
├── docker/               # Configurações Docker
├── kubernetes/           # Manifests Kubernetes
├── terraform/            # Infrastructure as Code
├── scripts/              # Scripts utilitários
└── docs/                 # Documentação
```

## Pré-requisitos

- .NET 10 SDK
- Node.js 18+ e npm
- Docker e Docker Compose
- PostgreSQL 15+ (ou usar via Docker)
- Redis (ou usar via Docker)

## Instalação e Execução

### Desenvolvimento Local

#### Backend

```bash
cd backend/src
dotnet restore
dotnet build
dotnet run --project WMS.API
```

A API estará disponível em `http://localhost:5000`

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

O frontend estará disponível em `http://localhost:5173`

### Usando Docker Compose

```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar todos os serviços
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

Serviços disponíveis:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Grafana**: http://localhost:3001 (admin/admin)
- **Prometheus**: http://localhost:9090
- **PgAdmin**: http://localhost:5050 (admin@wms.com/admin)
- **Elasticsearch**: http://localhost:9200

## Testes

### Backend

```bash
cd backend

# Testes unitários
dotnet test tests/WMS.UnitTests

# Testes de integração
dotnet test tests/WMS.IntegrationTests

# Testes E2E
dotnet test tests/WMS.E2ETests

# Todos os testes com coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend

```bash
cd frontend

# Testes unitários
npm run test

# Testes com UI
npm run test:ui

# Coverage
npm run test:coverage
```

## Scripts Úteis

### Backend

```bash
# Restaurar dependências
dotnet restore

# Build
dotnet build

# Executar migrações
dotnet ef database update --project src/WMS.Infrastructure

# Criar nova migração
dotnet ef migrations add MigrationName --project src/WMS.Infrastructure
```

### Frontend

```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build de produção
npm run build

# Preview do build
npm run preview

# Lint
npm run lint

# Format
npm run format
```

## Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

- **Domain Layer**: Entidades, Value Objects, Domain Events
- **Application Layer**: Use Cases, DTOs, Interfaces
- **Infrastructure Layer**: Implementações de persistência, messaging, cache
- **API Layer**: Controllers, Middleware, Configuration

## Módulos Principais

- **Inbound**: Recebimento de mercadorias
- **Picking**: Separação de pedidos
- **Packing**: Embalagem
- **Shipping**: Expedição
- **Inventory**: Gestão de estoque
- **Returns**: Devoluções
- **Reports**: Relatórios e dashboards

## Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## Licença

Copyright © 2025 WMS Interprise. Todos os direitos reservados.

## Suporte

Para questões e suporte, entre em contato através das issues do GitHub.

# Análise da Reestruturação para .NET 10

## 📊 Contexto Atual do Projeto

### ✅ Migração Concluída: Go → .NET 10

**Data de Análise**: 16 de novembro de 2025  
**Status**: Arquitetura .NET em camadas pronta, aguardando implementações

---

## 🏗️ Arquitetura em Camadas (.NET)

```
WMS.API                     (Camada de Apresentação)
  ├── Program.cs           (Apenas WeatherForecast placeholder)
  ├── appsettings.json     (Configuração completa)
  └── Controllers/         (Ainda não criada)

WMS.Application             (Camada de Aplicação)
  ├── DTOs/                (Requests/Responses)
  │   ├── LoginRequest     ✅ Criado
  │   ├── RegisterUserRequest ✅ Criado
  │   ├── ChangePasswordRequest ✅ Criado
  │   └── ...
  ├── Services/            (Interfaces apenas - sem implementação)
  │   ├── IAuthenticationService ✅ Definida
  │   ├── IUserService ✅ Definida
  │   └── IAuditService ✅ Definida
  ├── UseCases/            (Ainda vazio)
  ├── Validators/          (Ainda vazio)
  └── Mappings/            (AutoMapper configs)

WMS.Domain                  (Camada de Domínio)
  ├── Entities/            (Totalmente definidas)
  │   ├── BaseEntity       ✅ (com soft delete + audit)
  │   ├── User             ✅ (com business logic)
  │   ├── Role             ✅ Definida
  │   ├── Permission       ✅ Definida
  │   ├── Tenant           ✅ Definida
  │   ├── AuditLog         ✅ Definida
  │   ├── UserRole         ✅ Definida (join table)
  │   └── RolePermission   ✅ Definida (join table)
  ├── Enums/               (Tipos de negócio)
  ├── ValueObjects/        (Padrões de domínio)
  ├── Events/              (Domain events)
  └── Exceptions/          (Exceções de negócio)

WMS.Infrastructure          (Camada de Infraestrutura)
  ├── Persistence/         (EF Core + Repositórios)
  │   ├── ApplicationDbContext ✅ (Configurado com soft delete)
  │   ├── Configurations/  ✅ (Fluent API para cada entidade)
  │   │   ├── UserConfiguration
  │   │   ├── RoleConfiguration
  │   │   ├── PermissionConfiguration
  │   │   ├── TenantConfiguration
  │   │   ├── UserRoleConfiguration
  │   │   ├── RolePermissionConfiguration
  │   │   └── AuditLogConfiguration
  │   ├── Migrations/      (Vazio - precisa gerar)
  │   ├── Repositories/    (Ainda vazio)
  │   └── Seeds/           (Dados iniciais)
  ├── Identity/            (JWT, autenticação) - VAZIO
  ├── Caching/             (Redis) - VAZIO
  ├── Messaging/           (Kafka) - VAZIO
  ├── Search/              (Elasticsearch) - VAZIO
  ├── Logging/             (Serilog) - VAZIO
  ├── ExternalServices/    (APIs externas) - VAZIO
  ├── Security/            (Encryption, hashing) - VAZIO
  └── Storage/             (Blob storage) - VAZIO

WMS.Shared                  (Utilitários compartilhados)
  └── Ainda a analisar
```

---

## ✅ Implementações Concluídas

### 1. **Domain Layer** (100% ✅)
- Entidades com soft delete (IsDeleted + DeletedAt)
- BaseEntity com auditoria (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- User com lógica de negócio:
  - Lockout após 5 tentativas de login
  - Refresh token com expiry
  - Validação de email
  - Métodos para AddRole, RemoveRole
  - GetFullName(), UpdateProfile(), etc.
- Relacionamentos configurados (User ↔ Role ↔ Permission)
- Tenant support (multi-tenancy)

### 2. **Application DTOs** (90% ✅)
- LoginRequest, RegisterUserRequest, ChangePasswordRequest ✅
- Service interfaces (IAuthenticationService, IUserService, IAuditService) ✅
- Faltam implementações dos services

### 3. **Infrastructure/Persistence** (70% ✅)
- ApplicationDbContext com global query filters para soft delete ✅
- Fluent API configurations para todas as entidades ✅
- Índices definidos (Username, Email, RefreshToken, TenantId, etc.) ✅
- Cascade delete policies configuradas ✅
- AutoSave changes com auditoria ✅
- Migrations folder criada (vazia - precisa gerar)

### 4. **Configuration** (100% ✅)
- appsettings.json com todos os settings necessários:
  - JWT settings (SecretKey, Issuer, Audience, Expiration)
  - Connection strings (PostgreSQL + Redis)
  - CORS configurado para frontend (5173, 3000)
  - Kafka, Elasticsearch, Serilog
  - Multi-tenancy enabled
  - Rate limiting configurado

---

## ⏳ Implementações Faltando

### 1. **Migrations** (URGENTE)
```powershell
cd backend/src/WMS.Infrastructure
dotnet ef migrations add InitialCreate -o Persistence/Migrations
dotnet ef database update
```

### 2. **Service Implementations** (ALTA PRIORIDADE)
- [ ] AuthenticationService (login, register, refresh token, logout)
- [ ] UserService (CRUD operations)
- [ ] AuditService (logging de ações)

### 3. **Controllers** (MÉDIA PRIORIDADE)
- [ ] AuthenticationController (POST /api/auth/login, /register, /refresh-token, /logout)
- [ ] UsersController (GET, POST, PUT, DELETE /api/users/*)
- [ ] RolesController (GET, POST, PUT, DELETE /api/roles/*)
- [ ] PermissionsController (GET, POST, PUT, DELETE /api/permissions/*)
- [ ] AuditController (GET /api/audit/*)

### 4. **Security & Cryptography** (ALTA PRIORIDADE)
- [ ] JWT Token Service
- [ ] Password Hashing Service (BCrypt)
- [ ] Encryption Service

### 5. **Middleware** (MÉDIA PRIORIDADE)
- [ ] JWT Authentication middleware
- [ ] Authorization middleware
- [ ] Exception handling middleware
- [ ] Logging middleware
- [ ] Multi-tenancy middleware

### 6. **Repositories** (ALTA PRIORIDADE)
- [ ] IUserRepository
- [ ] IRoleRepository
- [ ] IPermissionRepository
- [ ] IAuditLogRepository
- [ ] Implementações concretas (GenericRepository pattern)

### 7. **Infrastructure Services** (BAIXA PRIORIDADE - Phase 2)
- [ ] Redis caching
- [ ] Kafka messaging
- [ ] Elasticsearch logging
- [ ] Email service (SMTP)
- [ ] File storage

---

## 🔧 Stack Tecnológico Confirmado

### Backend (.NET 10)
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### Banco de Dados
- PostgreSQL 15 (appsettings: localhost:5432/WMS_Interprise)
- EF Core 8 (InMemory para testes)
- Migrations support

### Packages esperados:
```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
Serilog
StackExchange.Redis
Confluent.Kafka
Elasticsearch.Net
FluentValidation
AutoMapper
```

---

## 🚀 Próximos Passos Recomendados

### Fase 1: Setup Essencial (1-2 horas)
1. ✅ Instalar packages necessários
2. ✅ Gerar inicial migration
3. ✅ Implementar services de autenticação
4. ✅ Criar AuthenticationController
5. ✅ Implementar middleware JWT
6. ✅ Testar login com admin@wms.local

### Fase 2: Core Features (2-3 horas)
1. ✅ Implementar UserService + UserController
2. ✅ Implementar RoleService + RoleController
3. ✅ Implementar PermissionService + PermissionController
4. ✅ Implementar AuditService + AuditController
5. ✅ Criar repositories

### Fase 3: Infrastructure (1-2 horas)
1. ✅ Redis caching
2. ✅ Kafka messaging
3. ✅ Elasticsearch logging
4. ✅ Email service

### Fase 4: Frontend Integration (1 hora)
1. ✅ Atualizar axios client para novos endpoints
2. ✅ Testar login flow completo
3. ✅ Validar admin pages

---

## 📝 Notas de Contexto

### Diferenças Go vs .NET

| Aspecto | Go | .NET 10 |
|---------|-----|---------|
| Framework | Gin | ASP.NET Core Minimal APIs |
| ORM | pgx/sql | EF Core |
| Testing | testing, testify | xUnit, Moq |
| Dependency Injection | Manual | Built-in |
| Middleware | Gin middleware | ASP.NET middleware |
| Configuration | .env | appsettings.json |
| Deployment | Binary | Docker container |

### Estrutura de Entidades Melhorada
- **Go anterior**: Schemas simples em SQL
- **.NET atual**: Rich domain model com business logic na entidade
- Exemplo: `User.AddRole()`, `User.IsLockedOut()`, `User.RecordFailedLogin()`

### Multi-tenancy
- ✅ Suportado nativamente via `TenantId` em entidades
- ✅ Global query filters aplicados automaticamente
- ✅ Middleware de multi-tenancy a implementar

---

## 📦 Estrutura de Pastas Confirmada

```
backend/
├── src/
│   ├── WMS.API/                    (Entry point, Controllers)
│   ├── WMS.Application/            (Services, DTOs, UseCases)
│   ├── WMS.Domain/                 (Entities, Interfaces, ValueObjects)
│   ├── WMS.Infrastructure/         (EF Core, Repositories, External Services)
│   ├── WMS.Shared/                 (Common utilities)
│   └── WMS.sln                     (Solution file)
├── tests/
│   ├── WMS.UnitTests/
│   ├── WMS.IntegrationTests/
│   └── WMS.E2ETests/
├── migrations/                     (SQL scripts - optional, EF managed)
├── Dockerfile
├── global.json                     (.NET SDK version)
└── Directory.Build.props           (Build settings)
```

---

## ⚠️ Configurações Críticas

### appsettings.json
- ✅ JWT Secret: "your-secret-key-here-change-in-production"
- ✅ DB Connection: PostgreSQL em localhost:5432
- ✅ CORS: http://localhost:5173 (frontend React)
- ✅ Multi-tenancy: Enabled
- ✅ Serilog: Configurado com Console + File + Elasticsearch

### Entidade User - Campos Especiais
- `IsLockedOut()`: Bloqueia após 5 falhas
- `RefreshToken`: Expiração separada
- `FailedLoginAttempts`: Contador de tentativas
- `EmailConfirmed`: Validação de email
- Soft delete: via `IsDeleted` + `DeletedAt`

---

**Status Geral**: 🟡 60% Completo - Arquitetura pronta, implementações iniciadas

Aguardando desenvolvimento das camadas de serviço, controllers e middleware.

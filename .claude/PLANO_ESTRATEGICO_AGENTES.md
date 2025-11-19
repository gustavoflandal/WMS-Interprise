# Plano Estratégico de Desenvolvimento WMS-Interprise - Fase 2
## Utilização Otimizada de Agentes Especializados

**Data:** 19 de novembro de 2025
**Branch:** `dev/fase2-operacoes`
**Objetivo:** Implementar módulos de operação principal (Recebimento, Armazenagem, Picking) com qualidade enterprise

---

## 1. Análise da Situação Atual

### 1.1 Status do Projeto

**Completo (70-75%):**
- ✅ Autenticação e autorização (JWT + RBAC)
- ✅ Estrutura de Clean Architecture + DDD
- ✅5 dados mestres (Users, Roles, Permissions, Companies, Warehouses)
- ✅ 1 módulo operacional básico (Customer - em conclusão)
- ✅ Frontend responsivo com Material-UI
- ✅ Documentação abrangente (14+ documentos)
- ✅ Docker Compose com infraestrutura completa

**Em Progresso (5-10%):**
- 🔄 Customer module (últimos ajustes e testes)
- 🔄 Limpeza de migrações duplicadas
- 🔄 Refinamento de validações

**Pendente (15-20%):**
- ❌ Recebimento de Mercadorias (RF-001)
- ❌ Armazenagem e Alocação (RF-002)
- ❌ Picking e Consolidação (RF-003)
- ❌ Testes unitários e integração
- ❌ CI/CD pipeline

### 1.2 Problemas Identificados

**Crítico:**
1. Duplicação de migrations de Customer
2. Ausência completa de testes automatizados

**Importante:**
3. Falta de validações em alguns DTOs
4. Error handling não padronizado
5. Documentação de API (Swagger) ausente

---

## 2. Estratégia de Utilização de Agentes

### 2.1 Especialidades Disponíveis

```
┌─────────────────────────────────────────┐
│     AGENTES ESPECIALIZADOS DO CLAUDE    │
├─────────────────────────────────────────┤
│ 1. general-purpose                      │
│    → Pesquisa complexa                  │
│    → Busca de código                    │
│    → Tarefas multi-step                 │
├─────────────────────────────────────────┤
│ 2. Explore                              │
│    → Análise de codebase                │
│    → Busca de padrões                   │
│    → Entendimento arquitetural          │
├─────────────────────────────────────────┤
│ 3. claude-code-guide                    │
│    → Documentação Claude Code           │
│    → Features e hooks                   │
│    → Claude Agent SDK                   │
└─────────────────────────────────────────┘
```

### 2.2 Mapeamento de Tarefas × Agentes

| Fase | Tarefa | Agente Recomendado | Justificativa |
|------|--------|-------------------|--------------|
| **Limpeza** | Remover migrations duplicadas | general-purpose | Busca, análise e edição automática |
| **Backend** | Estrutura Recebimento (RF-001) | Nenhum (Manual) | Requer decisões arquiteturais |
| **Backend** | Estrutura Armazenagem (RF-002) | Nenhum (Manual) | Algoritmos complexos |
| **Backend** | Estrutura Picking (RF-003) | Nenhum (Manual) | Lógica de negócio crítica |
| **Validações** | Adicionar Data Annotations | general-purpose | Busca e padronização |
| **Testes** | Implementar testes unitários | Nenhum (Manual) | Requer conhecimento de domínio |
| **Frontend** | Componentes UI Operação | Nenhum (Manual) | Decisões de UX/design |
| **DevOps** | CI/CD GitHub Actions | general-purpose | Criação de workflows |

---

## 3. Roteiro de Desenvolvimento - Fase 2

### 3.1 Sprint 1: Consolidação e Limpeza (1-2 dias)

**Objetivo:** Base sólida para novos desenvolvimentos

#### Task 1: Resolver duplicação de migrations

```
├─ Identificar migrations duplicadas de Customer
├─ Manter migração com implementação correta
├─ Remover migração vazia
├─ Validar schema gerado
└─ Testar rollback/forward
```

**Arquivos Afetados:**
- `backend/src/WMS.Infrastructure/Migrations/20251119120000_AddCustomerTable.cs` (REMOVER)
- `backend/src/WMS.Infrastructure/Migrations/20251119144724_AddCustomerTableMigration.cs` (MANTER)
- `backend/src/WMS.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs` (UPDATE)

#### Task 2: Padronizar validações em DTOs

```
├─ Adicionar [Required] em campos obrigatórios
├─ Adicionar [MaxLength] em strings
├─ Adicionar [EmailAddress] em emails
├─ Adicionar [Phone] em telefones
├─ Adicionar [CreditCard] para CPF/CNPJ customizado
└─ Testes de validação
```

**Scope:** Todos os DTOs em `WMS.Application/DTOs/`

#### Task 3: Implementar padrão de erro global

```
├─ Criar ErrorResponse dto padronizado
├─ Implementar exception filter global
├─ Padronizar todas as respostas de erro
├─ Documentar códigos de erro
└─ Integração no frontend
```

**Output:**
- Todas as APIs retornam: `{ statusCode, message, errors, timestamp }`

---

### 3.2 Sprint 2: Módulo de Recebimento (RF-001) (3-4 dias)

**Objetivo:** Implementar fluxo de recebimento de mercadorias

#### Entidades a Criar

```csharp
namespace WMS.Domain.Entities
{
    // Notificação de chegada
    public class ASN (Advance Shipping Notice)
    {
        public int Id
        public string AsnNumber                  // ID externo
        public int WarehouseId
        public int ProviderId                   // Fornecedor/Transportador
        public DateTime ScheduledArrivalDate
        public DateTime? ActualArrivalDate
        public ASNStatus Status                 // Scheduled, InTransit, Arrived, Received
        public int ExpectedItemCount
        public decimal ExpectedWeight
        public string DocumentNumber            // NF
        public int CreatedBy
        public DateTime CreatedAt
        // ... soft delete, audit
    }

    // Item individual da ASN
    public class ASNItem
    {
        public int Id
        public int AsnId
        public string Sku
        public int ExpectedQuantity
        public string Unit
        public decimal? ExpectedWeight
        public DateTime? ExpiryDate
        public string LotNumber
        public int? ReceivedQuantity
        public bool IsConformed
    }

    // Recebimento efetivo
    public class ReceiptDocumentation
    {
        public int Id
        public int AsnId
        public int WarehouseId
        public string ReceiptNumber
        public DateTime ReceiptDate
        public ReceiptStatus Status              // Draft, Confirmed, Closed
        public decimal TotalQuantity
        public int OperatorId                   // Quem recebeu
        public string Notes
        public List<ReceiptItem> Items
        // ... soft delete, audit
    }

    // Item recebido
    public class ReceiptItem
    {
        public int Id
        public int ReceiptDocumentationId
        public string Sku
        public int QuantityReceived
        public decimal? ActualWeight
        public string LotNumber
        public DateTime? ExpiryDate
        public int StorageLocationId            // Onde será armazenado
        public string QualityStatus             // Accepted, Rejected, PartiallyAccepted
        public string RejectionReason
    }

    // Localização de armazenamento
    public class StorageLocation
    {
        public int Id
        public int WarehouseId
        public string Code                      // A-001-01-01 (Corredor-Estante-Prateleira-Posição)
        public StorageZone Zone                 // Picking, Reserve, Cross-Dock, Quarantine
        public LocationStatus Status            // Available, Occupied, Unavailable
        public int? CurrentSkuId
        public int CurrentQuantity
        public decimal MaxCapacityKg
        public int MaxCapacityUnits
        public int RowPosition
        public int ColumnPosition
        public int LevelPosition
        public List<StorageLocationAttribute> Attributes  // Temperatura, Humidade, etc
    }

    // Atributo de localização (temperatura, etc)
    public class StorageLocationAttribute
    {
        public int Id
        public int StorageLocationId
        public string AttributeType             // Temperature, Humidity, LightExposure
        public string Value
        public string Unit
    }

    // SKU / Produto
    public class Product
    {
        public int Id
        public string Sku                       // Código único
        public string Name
        public string Description
        public ProductCategory Category         // Seco, Refrigerado, Congelado, Perecível, etc
        public ProductType Type                 // Commodity, Fractionable, Fragile
        public decimal UnitWeight
        public decimal UnitVolume
        public int DefaultWarehouseZone         // Picking, Reserve, etc
        public bool RequiresLotTracking
        public bool RequiresSerialNumber
        public DateTime? ShelfLife
        public bool IsActive
        // ... soft delete, audit
    }
}
```

#### Serviços a Criar

```csharp
namespace WMS.Application.Services
{
    public interface IASNService
    {
        Task<ASNResponse> CreateAsync(CreateASNRequest request);
        Task<ASNResponse> GetByNumberAsync(string asnNumber);
        Task<IEnumerable<ASNResponse>> GetPendingAsync(int warehouseId);
        Task<ASNResponse> UpdateStatusAsync(int asnId, ASNStatus newStatus);
        Task<IEnumerable<ASNItemResponse>> GetItemsAsync(int asnId);
    }

    public interface IReceiptService
    {
        Task<ReceiptDocumentationResponse> CreateAsync(CreateReceiptRequest request);
        Task<ReceiptDocumentationResponse> GetAsync(int receiptId);
        Task<ReceiptDocumentationResponse> AddItemAsync(int receiptId, ReceiptItemRequest item);
        Task<ReceiptDocumentationResponse> ConfirmAsync(int receiptId);
        Task<ReceiptDocumentationResponse> RejectItemAsync(int receiptId, int itemId, string reason);
        Task<IEnumerable<ReceiptDocumentationResponse>> GetByWarehouseAsync(int warehouseId, DateOnly date);
    }

    public interface IStorageLocationService
    {
        Task<StorageLocationResponse> AllocateLocationAsync(AllocationRequest request);
        Task<IEnumerable<StorageLocationResponse>> GetAvailableLocationsAsync(
            int warehouseId,
            ProductCategory category,
            int requiredCapacity);
        Task<StorageLocationResponse> UpdateCapacityAsync(int locationId, int quantityReceived);
        Task<IEnumerable<StorageLocationResponse>> GetByZoneAsync(int warehouseId, StorageZone zone);
    }

    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request);
        Task<ProductResponse> GetBySKUAsync(string sku);
        Task<IEnumerable<ProductResponse>> GetByWarehouseAsync(int warehouseId);
        Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
    }
}
```

#### Controllers a Criar

```csharp
// ASNController.cs
[ApiController]
[Route("api/[controller]")]
public class ASNController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ASNResponse>> Create(CreateASNRequest request);

    [HttpGet("{asnNumber}")]
    public async Task<ActionResult<ASNResponse>> GetByNumber(string asnNumber);

    [HttpGet("warehouse/{warehouseId}/pending")]
    public async Task<ActionResult<IEnumerable<ASNResponse>>> GetPending(int warehouseId);

    [HttpPut("{asnId}/status")]
    public async Task<ActionResult<ASNResponse>> UpdateStatus(int asnId, UpdateASNStatusRequest request);
}

// ReceiptController.cs
[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReceiptDocumentationResponse>> Create(CreateReceiptRequest request);

    [HttpGet("{receiptId}")]
    public async Task<ActionResult<ReceiptDocumentationResponse>> Get(int receiptId);

    [HttpPost("{receiptId}/items")]
    public async Task<ActionResult<ReceiptDocumentationResponse>> AddItem(int receiptId, ReceiptItemRequest request);

    [HttpPost("{receiptId}/confirm")]
    public async Task<ActionResult<ReceiptDocumentationResponse>> Confirm(int receiptId);

    [HttpPost("{receiptId}/items/{itemId}/reject")]
    public async Task<ActionResult> RejectItem(int receiptId, int itemId, string reason);
}

// StorageLocationController.cs
[ApiController]
[Route("api/[controller]")]
public class StorageLocationController : ControllerBase
{
    [HttpPost("allocate")]
    public async Task<ActionResult<StorageLocationResponse>> Allocate(AllocationRequest request);

    [HttpGet("warehouse/{warehouseId}/zone/{zone}/available")]
    public async Task<ActionResult<IEnumerable<StorageLocationResponse>>> GetAvailable(int warehouseId, string zone);
}

// ProductController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request);

    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductResponse>> GetBySKU(string sku);
}
```

#### DTOs a Criar

Serão criados em `WMS.Application/DTOs/` com Requests e Responses

#### Migrações

```bash
dotnet ef migrations add "AddReceivingModule" -p backend/src/WMS.Infrastructure
```

---

### 3.3 Sprint 3: Módulo de Armazenagem (RF-002) (2-3 dias)

**Objetivo:** Implementar lógica inteligente de alocação

#### Algoritmos de Alocação

```csharp
namespace WMS.Application.Services.Allocation
{
    public interface IAllocationStrategy
    {
        Task<AllocationResult> AllocateAsync(AllocationContext context);
    }

    // 1. Alocação ABC
    public class ABCAllocationStrategy : IAllocationStrategy
    {
        // Produtos de alto giro → Picking zone
        // Produtos de médio giro → Intermediate zone
        // Produtos de baixo giro → Reserve zone
    }

    // 2. Alocação por Correlação
    public class CorrelationAllocationStrategy : IAllocationStrategy
    {
        // Produtos frequentemente vendidos juntos → Proximidade
    }

    // 3. Alocação por Características
    public class CharacteristicAllocationStrategy : IAllocationStrategy
    {
        // Tamanho/peso → Estruturas adequadas
        // Temperatura → Zona apropriada
        // Fragilidade → Altura otimizada
    }

    // 4. Alocação por Densidade
    public class DensityAllocationStrategy : IAllocationStrategy
    {
        // Maior proximidade ao ponto de consolidação
    }
}
```

#### Serviços a Criar

```csharp
public interface IStorageAllocationService
{
    Task<AllocationResult> AllocateProductAsync(
        int warehouseId,
        string sku,
        int quantity,
        AllocationStrategy strategy);

    Task<IEnumerable<RebalanceRecommendation>> GetRebalancingNeedsAsync(int warehouseId);

    Task<bool> RebalanceAsync(int warehouseId, IEnumerable<int> locationIds);
}

public interface IInventoryService
{
    Task<InventorySnapshot> GetCurrentStateAsync(int warehouseId);
    Task<IEnumerable<InventoryTransaction>> GetMovementHistoryAsync(
        int warehouseId,
        DateRange dateRange);
    Task<InventoryAging> GetAgingAnalysisAsync(int warehouseId, string sku);
}
```

---

### 3.4 Sprint 4: Módulo de Picking (RF-003) (3-4 dias)

**Objetivo:** Implementar fluxo de picking e consolidação

#### Entidades a Criar

```csharp
public class Order
{
    public int Id
    public string OrderNumber
    public int CustomerId
    public int WarehouseId
    public OrderStatus Status              // Pending, Picking, Consolidated, Shipped
    public List<OrderLine> Lines
    public int? AssignedZoneId
    public DateTime OrderDate
    public DateTime? TargetShipDate
}

public class OrderLine
{
    public int Id
    public int OrderId
    public string Sku
    public int QuantityRequested
    public int? QuantityPicked
    public int? QuantityPacked
    public int? QuantityShipped
    public StorageLocation PickingLocation
}

public class PickingTask
{
    public int Id
    public int OrderId
    public int AssignedPickerId
    public PickingStrategy Strategy         // Single-line, Batch, Zone, Wave
    public PickingTaskStatus Status
    public DateTime CreatedAt
    public DateTime? CompletedAt
    public int ItemsCount
    public int ItemsCompleted
}

public class PickingLine
{
    public int Id
    public int PickingTaskId
    public int OrderLineId
    public int StorageLocationId
    public int QuantityToPick
    public int? QuantityPicked
    public DateTime? PickedAt
    public int? PickedBy
}

public class ConsolidationUnit
{
    public int Id
    public string ConsolidationNumber
    public int WarehouseId
    public ConsolidationStatus Status
    public List<Order> Orders
    public List<ConsolidationBox> Boxes
    public DateTime CreatedAt
}

public class ConsolidationBox
{
    public int Id
    public int ConsolidationUnitId
    public string BoxNumber
    public decimal Weight
    public decimal Volume
    public List<BoxItem> Items
}

public class BoxItem
{
    public int Id
    public int ConsolidationBoxId
    public int OrderLineId
    public int Quantity
}
```

#### Serviços a Criar

```csharp
public interface IPickingService
{
    Task<PickingTaskResponse> CreatePickingTaskAsync(CreatePickingTaskRequest request);
    Task<PickingTaskResponse> GetAssignedToUserAsync(int userId, int warehouseId);
    Task<PickingLineResponse> PickLineAsync(int pickingLineId, int quantityPicked);
    Task<PickingTaskResponse> CompleteTaskAsync(int pickingTaskId);
    Task<IEnumerable<PickingTaskResponse>> GetPendingAsync(int warehouseId);
}

public interface IConsolidationService
{
    Task<ConsolidationUnitResponse> CreateAsync(CreateConsolidationRequest request);
    Task<ConsolidationUnitResponse> AddOrderAsync(int consolidationId, int orderId);
    Task<ConsolidationUnitResponse> CreateBoxAsync(int consolidationId, CreateBoxRequest request);
    Task<ConsolidationUnitResponse> FinalizeAsync(int consolidationId);
}

public interface IPickingStrategyService
{
    // Single-line: Uma linha por vez
    Task<IEnumerable<PickingTask>> CreateSingleLineTasksAsync(int warehouseId, DateOnly date);

    // Batch: Múltiplas linhas do mesmo SKU
    Task<IEnumerable<PickingTask>> CreateBatchTasksAsync(int warehouseId, int batchSize);

    // Zone: Por zona do armazém
    Task<IEnumerable<PickingTask>> CreateZoneTasksAsync(int warehouseId);

    // Wave: Por padrão de demanda
    Task<IEnumerable<PickingTask>> CreateWaveTasksAsync(int warehouseId, WaveDefinition definition);
}
```

---

### 3.5 Sprint 5: Testes e CI/CD (2-3 dias)

**Objetivo:** Garantir qualidade e automação

#### Testes Unitários

```
Backend/Tests/
├─ WMS.Application.Tests
│  ├─ Services
│  │  ├─ ReceiptServiceTests.cs
│  │  ├─ AllocationServiceTests.cs
│  │  ├─ PickingServiceTests.cs
│  │  └─ ConsolidationServiceTests.cs
│  └─ Validators
│     └─ CreateReceiptRequestValidatorTests.cs
│
└─ WMS.Infrastructure.Tests
   ├─ Repositories
   │  ├─ ReceiptRepositoryTests.cs
   │  └─ StorageLocationRepositoryTests.cs
   └─ Persistence
      └─ ApplicationDbContextTests.cs
```

#### Testes de Integração

```
├─ ReceiptControllerIntegrationTests.cs
├─ PickingControllerIntegrationTests.cs
└─ StorageLocationControllerIntegrationTests.cs
```

#### CI/CD Pipeline

```yaml
# .github/workflows/build-and-test.yml
name: Build and Test

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test

  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: npm ci
      - run: npm run build
      - run: npm run test
```

---

## 4. Matriz de Responsabilidades (RACI)

| Tarefa | Implementação | Review | Teste | Deploy |
|--------|---------------|--------|-------|--------|
| Duplicação Migrations | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| Validações DTOs | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| Error Handler | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| RF-001 (Recebimento) | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| RF-002 (Armazenagem) | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| RF-003 (Picking) | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| Frontend Operacional | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| Testes Unitários | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |
| CI/CD GitHub | 👤 Dev | 🔍 Dev | 🧪 Dev | 📦 Dev |

Legenda: 👤 Responsável | 🔍 Revisor | 🧪 Tester | 📦 Approver

---

## 5. Métricas e Definição de Sucesso

### 5.1 Métricas Técnicas

| Métrica | Meta | Medição |
|---------|------|---------|
| Code Coverage | ≥ 80% | SonarQube |
| Duplicação de Código | < 3% | SonarQube |
| Complexidade Ciclomática | < 10 | SonarQube |
| Performance - P95 Latency | < 500ms | APM |
| Uptime | ≥ 99.5% | Monitoring |

### 5.2 Métricas de Negócio

| Métrica | Meta | Medição |
|---------|------|---------|
| Tempo de Recebimento | 10 min/pallet | Operacional |
| Acurácia de Picking | ≥ 99.5% | QA |
| Eficiência de Armazenagem | ≥ 85% de utilização | Inventário |
| Tempo de Consolidação | 30 min/pedido | Operacional |

### 5.3 Definição de Pronto (DoD)

- [ ] Código escrito segue padrões estabelecidos
- [ ] Testes unitários implementados (cobertura ≥ 80%)
- [ ] Testes integração incluídos
- [ ] Code review aprovado (≥ 1 revisor)
- [ ] Documentação atualizada
- [ ] Commit message segue conventional commits
- [ ] Build passa em CI/CD
- [ ] Sem erros de linting/formatter
- [ ] Performance testada e aprovada

---

## 6. Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|--------|-----------|
| Complexidade algoritmos | Alta | Alto | Spike técnico prévio, pair programming |
| Performance queries | Média | Alto | Índices DB, N+1 prevention, caching |
| Mudança de requisitos | Baixa | Médio | Validação com stakeholders |
| Integração ERP | Média | Médio | Mock objects, adapter pattern |
| Deadline apertado | Alta | Médio | MVP scope, priorização |

---

## 7. Próximas Ações Imediatas

### 7.1 Hoje (Sprint Planning)

- [x] Criar branch `dev/fase2-operacoes`
- [x] Análise e documentação (este plano)
- [ ] Setup de ambiente
  - [ ] Validar docker-compose
  - [ ] Validar build backend
  - [ ] Validar build frontend
  - [ ] Validar migrations

### 7.2 Amanhã (Sprint 1 - Limpeza)

- [ ] **Task 1:** Resolver duplicação migrations
  - Remover `20251119120000_AddCustomerTable.cs`
  - Validar snapshot
  - Testar rollback

- [ ] **Task 2:** Adicionar validações DTOs
  - Todos os DTOs em `WMS.Application/DTOs/`
  - Criar validador Fluent para CNPJ/CPF customizado

- [ ] **Task 3:** Implementar error handler global
  - Exception filter
  - ErrorResponse dto
  - Swagger/Documentação

### 7.3 Fim de Semana (Sprint 2 - RF-001)

- [ ] Criar entidades (ASN, Receipt, StorageLocation, Product)
- [ ] Criar repositories
- [ ] Criar serviços
- [ ] Criar DTOs (Requests e Responses)
- [ ] Criar controllers
- [ ] Migrações
- [ ] Testes básicos

---

## 8. Dependências Externas

### 8.1 Bibliotecas Necessárias

```xml
<!-- Pode ser necessário adicionar: -->
<PackageReference Include="FluentValidation" Version="11.x" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.x" />
<PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="6.x" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="12.x" />
```

### 8.2 Configurações de Ambiente

```bash
# Docker compose já tem:
- PostgreSQL 16
- Redis 7
- Elasticsearch
- Kafka
- Prometheus/Grafana
- Jaeger
```

---

## 9. Referências de Documentação

1. **Requisitos Funcionais:** `documentos/02_Analise_Requisitos/02_REQUISITOS_FUNCIONAIS.md`
2. **Arquitetura:** `documentos/01_Visao_Geral/01_VISAO_GERAL_DO_PROJETO.md`
3. **Design DB:** `documentos/04_Design_Banco_Dados/`
4. **Clean Architecture:** Clean Architecture (Robert C. Martin)
5. **DDD:** Domain-Driven Design (Eric Evans)

---

## 10. Conclusão

Este plano estratégico define um roadmap claro para implementar os módulos operacionais críticos do WMS-Interprise (Recebimento, Armazenagem, Picking) com qualidade enterprise.

A utilização otimizada de agentes especializados para tarefas de busca, análise e automação mantém a produtividade elevada, enquanto as decisões arquiteturais críticas e lógica de negócio complexa permanecem com o desenvolvedor principal.

**O projeto está pronto para a Fase 2.**

---

**Elaborado por:** Claude Code Agent
**Data:** 19 de novembro de 2025
**Branch:** dev/fase2-operacoes
**Status:** ✅ Ready for Implementation

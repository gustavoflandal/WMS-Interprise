# Checklist de Desenvolvimento - WMS-Interprise Fase 2

**Data de Início:** 19 de novembro de 2025
**Branch:** `dev/fase2-operacoes`
**Sprint Duration:** 3-4 semanas (5 sprints)

---

## SPRINT 1: CONSOLIDAÇÃO E LIMPEZA
**Duração:** 1-2 dias | **Status:** 📅 Próximo

### Task 1.1: Remover Migração Duplicada de Customer
**Arquivo:** `backend/src/WMS.Infrastructure/Migrations/20251119120000_AddCustomerTable.cs`
**Prioridade:** 🔴 CRÍTICO

- [ ] Verificar conteúdo do arquivo (deve estar vazio ou incompleto)
- [ ] Confirmar que `20251119144724_AddCustomerTableMigration.cs` tem a implementação correta
- [ ] Remover arquivo `20251119120000_AddCustomerTable.cs`
- [ ] Validar `ApplicationDbContextModelSnapshot.cs`
- [ ] Testar rollback/forward de migrations
- [ ] Commit: `fix(migrations): Remove duplicated CustomerTable migration`

### Task 1.2: Padronizar Validações em DTOs
**Pasta:** `backend/src/WMS.Application/DTOs/`
**Prioridade:** 🟠 IMPORTANTE

- [ ] Adicionar `[Required]` em campos obrigatórios
- [ ] Adicionar `[StringLength(max)]` em strings
- [ ] Adicionar `[EmailAddress]` em campos email
- [ ] Adicionar `[Phone]` em campos telefone
- [ ] Criar validador customizado para CNPJ/CPF
- [ ] Adicionar `[Range]` onde apropriado
- [ ] Testar validação em endpoints
- [ ] Commit: `feat(validation): Add robust data annotations to all DTOs`

**DTOs a revisar:**
- [ ] CreateUserRequest
- [ ] UpdateUserRequest
- [ ] CreateCompanyRequest
- [ ] UpdateCompanyRequest
- [ ] CreateWarehouseRequest
- [ ] UpdateWarehouseRequest
- [ ] CreateCustomerRequest
- [ ] UpdateCustomerRequest
- [ ] LoginRequest
- [ ] RegisterUserRequest

### Task 1.3: Implementar Error Handler Global
**Arquivo:** `backend/src/WMS.API/Program.cs`
**Prioridade:** 🟠 IMPORTANTE

- [ ] Criar `Models/ErrorResponse.cs`:
  ```csharp
  public class ErrorResponse
  {
      public int StatusCode { get; set; }
      public string Message { get; set; }
      public Dictionary<string, string[]> Errors { get; set; }
      public DateTime Timestamp { get; set; }
  }
  ```

- [ ] Criar `Middleware/ExceptionHandlingMiddleware.cs`
  - [ ] Capturar todas as exceções
  - [ ] Logar com Serilog
  - [ ] Retornar ErrorResponse padronizado
  - [ ] Status codes apropriados (400, 404, 500)

- [ ] Criar `Filters/ValidationExceptionFilter.cs`
  - [ ] Capturar ValidationException
  - [ ] Retornar lista de erros detalhada

- [ ] Registrar em Program.cs
- [ ] Testar diferentes cenários de erro
- [ ] Documentar códigos de erro
- [ ] Commit: `feat(error-handling): Implement global exception handler`

**Testes obrigatórios:**
- [ ] POST com dados inválidos → 400 + ErrorResponse
- [ ] GET com ID inexistente → 404 + ErrorResponse
- [ ] Exceção não esperada → 500 + ErrorResponse
- [ ] Erro de validação → 400 + lista de errors

### Task 1.4: Validação de Build
**Prioridade:** 🟡 NORMAL

- [ ] `dotnet restore`
- [ ] `dotnet build` (sem warnings)
- [ ] `dotnet ef migrations list` (sem pendentes)
- [ ] `dotnet ef database update` (no Docker PostgreSQL)
- [ ] Verificar health endpoint: `GET http://localhost:5000/health`
- [ ] Frontend: `npm install && npm run build`

**Resultado esperado:** ✅ Build verde, sem erros nem warnings

---

## SPRINT 2: MÓDULO RECEBIMENTO (RF-001)
**Duração:** 3-4 dias | **Status:** 🔜 Próximo após Sprint 1

### Task 2.1: Criar Entidades de Domínio
**Pasta:** `backend/src/WMS.Domain/Entities/`
**Prioridade:** 🔴 CRÍTICO

#### 2.1.1 Product (SKU)
```csharp
public class Product : BaseEntity
{
    public string Sku { get; set; }                    // [Required, Index]
    public string Name { get; set; }                   // [Required]
    public string Description { get; set; }
    public ProductCategory Category { get; set; }      // Seco, Refrigerado, etc
    public ProductType Type { get; set; }              // Commodity, Fractionable, etc
    public decimal UnitWeight { get; set; }            // kg
    public decimal UnitVolume { get; set; }            // m³
    public int DefaultStorageZone { get; set; }        // Picking, Reserve, etc
    public bool RequiresLotTracking { get; set; }
    public bool RequiresSerialNumber { get; set; }
    public int? ShelfLifeDays { get; set; }
    public bool IsActive { get; set; }
    public int TenantId { get; set; }
}
```

- [ ] Implementar entidade
- [ ] Adicionar enums (ProductCategory, ProductType)
- [ ] Adicionar índices: `[Index(nameof(Sku))]`, `[Index(nameof(TenantId))]`
- [ ] Validações: SKU único por tenant, Weight/Volume > 0
- [ ] Commit: `feat(domain): Add Product entity and enums`

#### 2.1.2 StorageLocation
```csharp
public class StorageLocation : BaseEntity
{
    public int WarehouseId { get; set; }               // [Required, ForeignKey]
    public string Code { get; set; }                   // A-001-01-01 [Required, Index]
    public StorageZone Zone { get; set; }              // Picking, Reserve, Cross-Dock
    public LocationStatus Status { get; set; }         // Available, Occupied, Unavailable
    public int? CurrentProductId { get; set; }         // Se ocupado
    public int CurrentQuantity { get; set; }           // Quantidade atual
    public decimal MaxCapacityKg { get; set; }         // Capacidade máxima
    public int MaxCapacityUnits { get; set; }          // Unidades máximas
    public int RowPosition { get; set; }               // A (0), B (1), etc
    public int ColumnPosition { get; set; }            // 001
    public int LevelPosition { get; set; }             // 01
}
```

- [ ] Implementar entidade
- [ ] Adicionar enums (StorageZone, LocationStatus)
- [ ] Code debe ser único por warehouse
- [ ] Adicionar validações
- [ ] Commit: `feat(domain): Add StorageLocation entity`

#### 2.1.3 ASN (Advance Shipping Notice)
```csharp
public class ASN : BaseEntity
{
    public int WarehouseId { get; set; }
    public string AsnNumber { get; set; }              // [Required, Index, Unique]
    public int? ProviderId { get; set; }               // Fornecedor/Transportador
    public DateTime ScheduledArrivalDate { get; set; } // [Required]
    public DateTime? ActualArrivalDate { get; set; }
    public ASNStatus Status { get; set; }              // Scheduled, InTransit, Arrived
    public string DocumentNumber { get; set; }         // NF
    public int ExpectedItemCount { get; set; }
    public decimal ExpectedWeight { get; set; }
    public string Notes { get; set; }
    public List<ASNItem> Items { get; set; } = new();
}

public class ASNItem
{
    public int Id { get; set; }
    public int AsnId { get; set; }                     // [ForeignKey]
    public int ProductId { get; set; }                 // [ForeignKey]
    public int ExpectedQuantity { get; set; }          // [Required, > 0]
    public string Unit { get; set; }                   // UN, KG, etc
    public decimal? ExpectedWeight { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string LotNumber { get; set; }
    public int? ReceivedQuantity { get; set; }
    public bool IsConformed { get; set; }
}
```

- [ ] Implementar entidades
- [ ] Adicionar enum ASNStatus
- [ ] Índices: AsnNumber (Unique), WarehouseId, Status
- [ ] Validações
- [ ] Commit: `feat(domain): Add ASN and ASNItem entities`

#### 2.1.4 ReceiptDocumentation
```csharp
public class ReceiptDocumentation : BaseEntity
{
    public int WarehouseId { get; set; }
    public int? AsnId { get; set; }                    // Referência à ASN (opcional)
    public string ReceiptNumber { get; set; }          // [Required, Index, Unique]
    public DateTime ReceiptDate { get; set; }           // [Required]
    public ReceiptStatus Status { get; set; }           // Draft, Confirmed, Closed
    public decimal TotalQuantity { get; set; }
    public decimal TotalWeight { get; set; }
    public int OperatorId { get; set; }                // Quem recebeu [ForeignKey]
    public string Notes { get; set; }
    public List<ReceiptItem> Items { get; set; } = new();
}

public class ReceiptItem
{
    public int Id { get; set; }
    public int ReceiptDocumentationId { get; set; }    // [ForeignKey]
    public int ProductId { get; set; }                 // [ForeignKey]
    public int QuantityReceived { get; set; }          // [Required, > 0]
    public decimal ActualWeight { get; set; }
    public string LotNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int StorageLocationId { get; set; }         // Onde será armazenado [ForeignKey]
    public QualityStatus QualityStatus { get; set; }   // Accepted, Rejected, Partial
    public string RejectionReason { get; set; }
}
```

- [ ] Implementar entidades
- [ ] Adicionar enums (ReceiptStatus, QualityStatus)
- [ ] Índices
- [ ] Validações
- [ ] Commit: `feat(domain): Add ReceiptDocumentation and ReceiptItem entities`

### Task 2.2: Criar Configurações EF Core
**Pasta:** `backend/src/WMS.Infrastructure/Persistence/Configurations/`
**Prioridade:** 🔴 CRÍTICO

- [ ] ProductConfiguration.cs
  - [ ] Índices: Sku (unique por tenant), TenantId, IsActive
  - [ ] MaxLength: Sku(50), Name(200), Category(50), Type(50)
  - [ ] Relacionamentos

- [ ] StorageLocationConfiguration.cs
  - [ ] Índices: Code (unique por warehouse), WarehouseId, Zone, Status
  - [ ] MaxLength: Code(50)
  - [ ] Constraints: RowPosition ≥ 0, ColumnPosition ≥ 0, LevelPosition ≥ 0
  - [ ] Relacionamentos com Warehouse, Product

- [ ] ASNConfiguration.cs
  - [ ] Índices: AsnNumber (unique), WarehouseId, Status, ScheduledArrivalDate
  - [ ] MaxLength: AsnNumber(50), DocumentNumber(50)
  - [ ] Relacionamentos com Warehouse, User, ASNItem

- [ ] ReceiptDocumentationConfiguration.cs
  - [ ] Índices: ReceiptNumber (unique), WarehouseId, ReceiptDate, Status
  - [ ] MaxLength: ReceiptNumber(50)
  - [ ] Relacionamentos com Warehouse, User, ReceiptItem

- [ ] Commit: `feat(infrastructure): Add EF Core entity configurations`

### Task 2.3: Criar Repositories
**Pasta:** `backend/src/WMS.Infrastructure/Persistence/Repositories/`
**Prioridade:** 🔴 CRÍTICO

- [ ] IProductRepository.cs + ProductRepository.cs
  ```csharp
  Task<Product> GetBySKUAsync(string sku, int tenantId);
  Task<bool> SKUExistsAsync(string sku, int tenantId);
  Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, int tenantId);
  Task<IEnumerable<Product>> GetActiveAsync(int tenantId);
  ```

- [ ] IStorageLocationRepository.cs + StorageLocationRepository.cs
  ```csharp
  Task<StorageLocation> GetByCodeAsync(string code, int warehouseId);
  Task<IEnumerable<StorageLocation>> GetAvailableAsync(int warehouseId);
  Task<IEnumerable<StorageLocation>> GetByZoneAsync(int warehouseId, StorageZone zone);
  Task<IEnumerable<StorageLocation>> GetOccupiedAsync(int warehouseId);
  ```

- [ ] IASNRepository.cs + ASNRepository.cs
  ```csharp
  Task<ASN> GetByNumberAsync(string asnNumber, int warehouseId);
  Task<IEnumerable<ASN>> GetPendingAsync(int warehouseId);
  Task<IEnumerable<ASN>> GetByStatusAsync(int warehouseId, ASNStatus status);
  Task<IEnumerable<ASN>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to);
  ```

- [ ] IReceiptRepository.cs + ReceiptRepository.cs
  ```csharp
  Task<ReceiptDocumentation> GetByNumberAsync(string receiptNumber, int warehouseId);
  Task<IEnumerable<ReceiptDocumentation>> GetByWarehouseAsync(int warehouseId, DateTime date);
  Task<IEnumerable<ReceiptDocumentation>> GetByStatusAsync(int warehouseId, ReceiptStatus status);
  ```

- [ ] Register em UnitOfWork.cs
- [ ] Commit: `feat(infrastructure): Add repositories for receiving module`

### Task 2.4: Criar Serviços da Aplicação
**Pasta:** `backend/src/WMS.Application/Services/`
**Prioridade:** 🔴 CRÍTICO

- [ ] IProductService.cs + ProductService.cs
  ```csharp
  Task<ProductResponse> CreateAsync(CreateProductRequest request);
  Task<ProductResponse> GetBySKUAsync(string sku);
  Task<IEnumerable<ProductResponse>> GetAllAsync(int tenantId);
  Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
  Task<bool> DeleteAsync(int id);
  ```

- [ ] IASNService.cs + ASNService.cs
  ```csharp
  Task<ASNResponse> CreateAsync(CreateASNRequest request);
  Task<ASNResponse> GetByNumberAsync(string asnNumber, int warehouseId);
  Task<IEnumerable<ASNResponse>> GetPendingAsync(int warehouseId);
  Task<ASNResponse> UpdateStatusAsync(int asnId, ASNStatus newStatus);
  Task<IEnumerable<ASNItemResponse>> GetItemsAsync(int asnId);
  Task<ASNResponse> AddItemAsync(int asnId, CreateASNItemRequest item);
  ```

- [ ] IReceiptService.cs + ReceiptService.cs
  ```csharp
  Task<ReceiptDocumentationResponse> CreateAsync(CreateReceiptRequest request);
  Task<ReceiptDocumentationResponse> GetAsync(int receiptId);
  Task<ReceiptDocumentationResponse> GetByNumberAsync(string receiptNumber, int warehouseId);
  Task<ReceiptDocumentationResponse> AddItemAsync(int receiptId, CreateReceiptItemRequest item);
  Task<ReceiptDocumentationResponse> ConfirmAsync(int receiptId);
  Task<ReceiptDocumentationResponse> RejectItemAsync(int receiptId, int itemId, string reason);
  Task<IEnumerable<ReceiptDocumentationResponse>> GetByWarehouseAsync(int warehouseId, DateTime date);
  ```

- [ ] IStorageLocationService.cs + StorageLocationService.cs
  ```csharp
  Task<StorageLocationResponse> AllocateLocationAsync(int warehouseId, ProductCategory category, int requiredCapacity);
  Task<IEnumerable<StorageLocationResponse>> GetAvailableAsync(int warehouseId);
  Task<IEnumerable<StorageLocationResponse>> GetByZoneAsync(int warehouseId, StorageZone zone);
  Task<bool> UpdateCapacityAsync(int locationId, int quantityReceived);
  ```

- [ ] Adicionar validações de negócio
- [ ] Registrar em Program.cs
- [ ] Commit: `feat(application): Add services for receiving module`

### Task 2.5: Criar DTOs
**Pasta:** `backend/src/WMS.Application/DTOs/`
**Prioridade:** 🔴 CRÍTICO

**Requests:**
- [ ] CreateProductRequest
- [ ] UpdateProductRequest
- [ ] CreateASNRequest
- [ ] CreateASNItemRequest
- [ ] UpdateASNStatusRequest
- [ ] CreateReceiptRequest
- [ ] CreateReceiptItemRequest
- [ ] ConfirmReceiptRequest

**Responses:**
- [ ] ProductResponse
- [ ] ASNResponse
- [ ] ASNItemResponse
- [ ] ReceiptDocumentationResponse
- [ ] ReceiptItemResponse
- [ ] StorageLocationResponse

**Validações:**
- [ ] [Required] em campos obrigatórios
- [ ] [StringLength] em strings
- [ ] [Range] em números
- [ ] Commit: `feat(application): Add DTOs for receiving module`

### Task 2.6: Criar Controllers
**Pasta:** `backend/src/WMS.API/Controllers/`
**Prioridade:** 🔴 CRÍTICO

- [ ] ProductController.cs
  ```
  POST   /api/products                 (Create)
  GET    /api/products/:id             (GetById)
  GET    /api/products/sku/:sku        (GetBySKU)
  GET    /api/products                 (GetAll)
  PUT    /api/products/:id             (Update)
  DELETE /api/products/:id             (Delete)
  ```

- [ ] ASNController.cs
  ```
  POST   /api/asn                      (Create)
  GET    /api/asn/:asnNumber           (GetByNumber)
  GET    /api/asn/warehouse/:id/pending (GetPending)
  PUT    /api/asn/:id/status           (UpdateStatus)
  POST   /api/asn/:id/items            (AddItem)
  GET    /api/asn/:id/items            (GetItems)
  ```

- [ ] ReceiptController.cs
  ```
  POST   /api/receipts                 (Create)
  GET    /api/receipts/:id             (GetById)
  GET    /api/receipts/number/:number  (GetByNumber)
  POST   /api/receipts/:id/items       (AddItem)
  POST   /api/receipts/:id/confirm     (Confirm)
  POST   /api/receipts/:id/items/:itemId/reject (RejectItem)
  GET    /api/receipts/warehouse/:id/date/:date (GetByDate)
  ```

- [ ] StorageLocationController.cs
  ```
  GET    /api/storage-locations/warehouse/:id/available (GetAvailable)
  GET    /api/storage-locations/warehouse/:id/zone/:zone (GetByZone)
  POST   /api/storage-locations/allocate (AllocateLocation)
  ```

- [ ] Adicionar [Authorize] e verificação de tenant
- [ ] Commit: `feat(api): Add controllers for receiving module`

### Task 2.7: Criar Migration
**Prioridade:** 🔴 CRÍTICO

```bash
cd backend
dotnet ef migrations add "AddReceivingModule" -p src/WMS.Infrastructure
```

- [ ] Validar script SQL gerado
- [ ] Revisar indices criados
- [ ] Testar: `dotnet ef database update -p src/WMS.Infrastructure`
- [ ] Commit: `feat(migrations): Add ReceivingModule migration`

### Task 2.8: Atualizar AutoMapper Profile
**Arquivo:** `backend/src/WMS.Application/Mapping/MappingProfile.cs`
**Prioridade:** 🟡 NORMAL

- [ ] CreateMap<Product, ProductResponse>()
- [ ] CreateMap<CreateProductRequest, Product>()
- [ ] CreateMap<ASN, ASNResponse>()
- [ ] CreateMap<ASNItem, ASNItemResponse>()
- [ ] CreateMap<CreateASNRequest, ASN>()
- [ ] CreateMap<ReceiptDocumentation, ReceiptDocumentationResponse>()
- [ ] CreateMap<ReceiptItem, ReceiptItemResponse>()
- [ ] CreateMap<StorageLocation, StorageLocationResponse>()
- [ ] Commit: `feat(mapping): Add AutoMapper profiles for receiving`

### Task 2.9: Testes Básicos
**Pasta:** `backend/tests/WMS.Application.Tests/Services/`
**Prioridade:** 🟡 NORMAL

- [ ] ProductServiceTests.cs
  ```csharp
  [Test] void CreateProduct_ValidRequest_ReturnsSuccess()
  [Test] void CreateProduct_DuplicateSKU_ThrowsException()
  [Test] void GetBySKU_ExistingProduct_ReturnsProduct()
  [Test] void GetBySKU_NonExistingProduct_ReturnsNull()
  ```

- [ ] ReceiptServiceTests.cs
  ```csharp
  [Test] void CreateReceipt_ValidRequest_ReturnsSuccess()
  [Test] void AddItem_ValidItem_UpdatesReceipt()
  [Test] void Confirm_ReceiptWithItems_ChangesStatus()
  ```

- [ ] ASNServiceTests.cs
  ```csharp
  [Test] void CreateASN_ValidRequest_ReturnsSuccess()
  [Test] void GetPending_MultipleASN_ReturnsOnlyPending()
  ```

- [ ] Adicionar Moq para mocks
- [ ] Target: ≥ 80% coverage
- [ ] Commit: `test(receiving): Add unit tests for services`

### Task 2.10: Frontend - Página de Recebimento
**Pasta:** `frontend/src/pages/`
**Prioridade:** 🟡 NORMAL

- [ ] ReceivingPage.tsx - Página principal
- [ ] ASNListComponent - Listagem de ASNs
- [ ] ASNDetailComponent - Detalhe de ASN
- [ ] ReceiptFormComponent - Formulário de recebimento
- [ ] ProductSelectorComponent - Seletor de produtos
- [ ] StorageLocationSelectorComponent - Seletor de localização

Integração:
- [ ] Criar receiptApi.ts
- [ ] Criar productApi.ts
- [ ] Criar storageLocationApi.ts
- [ ] Adicionar types em types/api.ts
- [ ] Adicionar rota em App.tsx
- [ ] Commit: `feat(frontend): Add receiving module pages`

### Task 2.11: Validação Final Sprint 2
**Prioridade:** 🔴 CRÍTICO

- [ ] Build backend: `dotnet build` (sem warnings)
- [ ] Build frontend: `npm run build` (sem errors)
- [ ] Migrations: `dotnet ef database update`
- [ ] Testes: 80%+ coverage
- [ ] Swagger/API docs atualizado
- [ ] Manual testing:
  - [ ] POST produto
  - [ ] POST ASN
  - [ ] POST recebimento
  - [ ] GET com filtros
  - [ ] UPDATE status
- [ ] Frontend funcional:
  - [ ] Listar ASNs
  - [ ] Criar recebimento
  - [ ] Adicionar itens
  - [ ] Confirmar recebimento

**Resultado esperado:** ✅ Fluxo de recebimento 100% funcional

---

## SPRINT 3: MÓDULO ARMAZENAGEM (RF-002)
**Duração:** 2-3 dias | **Status:** 🔜 Após Sprint 2

### Resumo das Tasks:
- [ ] Implementar algoritmos de alocação (ABC, Correlação, Características, Densidade)
- [ ] IAllocationStrategy pattern
- [ ] StorageAllocationService com strategy factory
- [ ] InventoryService para gestão
- [ ] RebalancingService para otimização
- [ ] Controllers REST
- [ ] Frontend - Página de Armazenagem
- [ ] Testes
- [ ] Validação final

---

## SPRINT 4: MÓDULO PICKING (RF-003)
**Duração:** 3-4 dias | **Status:** 🔜 Após Sprint 3

### Resumo das Tasks:
- [ ] Entidades: Order, OrderLine, PickingTask, PickingLine, ConsolidationUnit
- [ ] Serviços: PickingService, PickingStrategyService, ConsolidationService
- [ ] Estratégias: Single-line, Batch, Zone, Wave
- [ ] Controllers REST
- [ ] DTOs com validações
- [ ] Migration
- [ ] Frontend - Página de Picking
- [ ] Testes
- [ ] Validação final

---

## SPRINT 5: TESTES E CI/CD
**Duração:** 2-3 dias | **Status:** 🔜 Após Sprint 4

### Resumo das Tasks:
- [ ] Cobertura de testes ≥ 80% (todos os módulos)
- [ ] Testes de integração (Controllers)
- [ ] E2E tests básicos
- [ ] GitHub Actions workflow
- [ ] SonarQube integration
- [ ] Documentação de API (Swagger)
- [ ] Performance testing
- [ ] Security scanning

---

## MÉTRICAS DE CONCLUSÃO

### Cada Sprint deve atingir:
- [ ] **Code Coverage:** ≥ 80%
- [ ] **Build Time:** < 2 minutos
- [ ] **Linting:** Zero warnings
- [ ] **Tests:** Todos passando
- [ ] **Documentation:** Atualizada
- [ ] **Code Review:** Aprovado

### Projeto como um todo:
- [ ] **Total Coverage:** ≥ 80%
- [ ] **Código duplicado:** < 3%
- [ ] **Complexidade:** < 10 (média)
- [ ] **API Latency:** P95 < 500ms
- [ ] **Uptime:** ≥ 99.5%

---

## RISCOS E MITIGAÇÕES

| Risco | Probabilidade | Mitigação |
|-------|---------------|-----------|
| Algoritmo complexo | Alta | Spike técnico, pair programming |
| Performance | Média | Índices DB, N+1 prevention |
| Mudança de req | Baixa | Validação com stakeholders |
| Escopo | Média | MVP, priorização |

---

## PRÓXIMAS AÇÕES

### ✅ Completadas hoje:
- [x] Análise do projeto
- [x] Criação de plano estratégico
- [x] Nova branch dev/fase2-operacoes
- [x] Documentação (PLANO_ESTRATEGICO_AGENTES.md, QUICK_REFERENCE.md)

### ⏭️ Próximas (Sprint 1):
- [ ] Remover migration duplicada de Customer
- [ ] Padronizar validações DTOs
- [ ] Implementar error handler global
- [ ] Validar build

### 📅 Timeline estimada:
- Sprint 1: 20-21 nov (1-2 dias)
- Sprint 2: 22-25 nov (3-4 dias)
- Sprint 3: 26-28 nov (2-3 dias)
- Sprint 4: 29-02 dez (3-4 dias)
- Sprint 5: 03-05 dez (2-3 dias)

---

**Última atualização:** 19 de novembro de 2025
**Prepared by:** Claude Code
**Status:** 🟢 READY FOR IMPLEMENTATION

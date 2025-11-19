# Quick Reference - WMS-Interprise Dev Setup

**Branch Atual:** `dev/fase2-operacoes`
**Data:** 19 de novembro de 2025
**Status:** 🟢 Ready for Development

---

## 1. Verificação Rápida de Ambiente

```bash
# Verificar .NET SDK
dotnet --version                    # Deve ser 10.0.x

# Verificar Node.js
node --version                      # Deve ser 18.x+
npm --version                       # Deve ser 9.x+

# Verificar Docker
docker --version
docker-compose --version

# Verificar PostgreSQL (via Docker)
docker ps                           # Listar containers ativos
```

---

## 2. Build & Run

### 2.1 Backend

```bash
# A partir de: D:\WMS-Interprise\backend

# Restaurar dependências
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/WMS.API/WMS.API.csproj

# Endpoints disponíveis:
# http://localhost:5000
# https://localhost:5001
```

### 2.2 Frontend

```bash
# A partir de: D:\WMS-Interprise\frontend

# Instalar dependências
npm install

# Dev server
npm run dev              # http://localhost:5173

# Build para produção
npm run build

# Testes
npm run test
```

### 2.3 Docker Compose (Infraestrutura)

```bash
# A partir de: D:\WMS-Interprise

# Subir todos os serviços
docker-compose up -d

# Verificar status
docker-compose ps

# Logs
docker-compose logs -f postgres          # PostgreSQL
docker-compose logs -f redis             # Redis
docker-compose logs -f pgadmin           # pgAdmin

# Parar serviços
docker-compose down
```

### 2.4 Database Migrations

```bash
# A partir de: D:\WMS-Interprise\backend

# Ver migrações pendentes
dotnet ef migrations list -p src/WMS.Infrastructure

# Aplicar migrações
dotnet ef database update -p src/WMS.Infrastructure

# Criar nova migração
dotnet ef migrations add "MigrationName" -p src/WMS.Infrastructure

# Remover última migração (se não foi aplicada)
dotnet ef migrations remove -p src/WMS.Infrastructure
```

---

## 3. Estrutura de Padrões

### 3.1 Criar Nova Entidade

**Passo 1:** Domain Layer
```csharp
// backend/src/WMS.Domain/Entities/MyEntity.cs
public class MyEntity : BaseEntity
{
    public string Name { get; set; }
    public MyEnum Status { get; set; }
    // Properties...
}
```

**Passo 2:** Repository Interface
```csharp
// backend/src/WMS.Domain/Interfaces/IMyRepository.cs
public interface IMyRepository : IRepository<MyEntity>
{
    Task<MyEntity> GetByNameAsync(string name);
    // Custom methods...
}
```

**Passo 3:** Repository Implementation
```csharp
// backend/src/WMS.Infrastructure/Persistence/Repositories/MyRepository.cs
public class MyRepository : Repository<MyEntity>, IMyRepository
{
    public MyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<MyEntity> GetByNameAsync(string name)
    {
        return await _context.Set<MyEntity>()
            .FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted);
    }
}
```

**Passo 4:** Register in UnitOfWork
```csharp
// backend/src/WMS.Infrastructure/Persistence/Repositories/UnitOfWork.cs
public IMyRepository MyRepository =>
    _myRepository ??= new MyRepository(_context);
```

**Passo 5:** Service Interface
```csharp
// backend/src/WMS.Application/Services/IMyService.cs
public interface IMyService
{
    Task<MyResponse> GetByIdAsync(int id);
    Task<MyResponse> CreateAsync(CreateMyRequest request);
    Task<MyResponse> UpdateAsync(int id, UpdateMyRequest request);
    Task<bool> DeleteAsync(int id);
}
```

**Passo 6:** Service Implementation
```csharp
// backend/src/WMS.Application/Services/MyService.cs
public class MyService : IMyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MyResponse> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.MyRepository.GetByIdAsync(id);
        if (entity == null)
            throw new NotFoundException($"MyEntity with id {id} not found");

        return _mapper.Map<MyResponse>(entity);
    }
    // Implement others...
}
```

**Passo 7:** Register in DI (Program.cs)
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

**Passo 8:** Create DTOs
```csharp
// backend/src/WMS.Application/DTOs/Requests/CreateMyRequest.cs
public class CreateMyRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public MyEnum Status { get; set; }
}

// backend/src/WMS.Application/DTOs/Responses/MyResponse.cs
public class MyResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MyEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Passo 9:** Create Controller
```csharp
// backend/src/WMS.API/Controllers/MyController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MyController : ControllerBase
{
    private readonly IMyService _myService;

    public MyController(IMyService myService)
    {
        _myService = myService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MyResponse>> GetById(int id)
    {
        var result = await _myService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MyResponse>> Create(CreateMyRequest request)
    {
        var result = await _myService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

**Passo 10:** Create Migration
```bash
dotnet ef migrations add "AddMyEntity" -p src/WMS.Infrastructure
dotnet ef database update -p src/WMS.Infrastructure
```

**Passo 11:** Create AutoMapper Profile (if not exists)
```csharp
// backend/src/WMS.Application/Mapping/MappingProfile.cs
CreateMap<MyEntity, MyResponse>();
CreateMap<CreateMyRequest, MyEntity>();
```

### 3.2 Criar Página Frontend

**Passo 1:** Create API Service
```typescript
// frontend/src/services/api/myApi.ts
import { httpClient } from './httpClient';
import { MyResponse, CreateMyRequest, UpdateMyRequest } from '@/types/api';

export const myApi = {
  getById: (id: number) =>
    httpClient.get<MyResponse>(`/api/my/${id}`),

  getAll: () =>
    httpClient.get<MyResponse[]>('/api/my'),

  create: (data: CreateMyRequest) =>
    httpClient.post<MyResponse>('/api/my', data),

  update: (id: number, data: UpdateMyRequest) =>
    httpClient.put<MyResponse>(`/api/my/${id}`, data),

  delete: (id: number) =>
    httpClient.delete(`/api/my/${id}`),
};
```

**Passo 2:** Create Types
```typescript
// frontend/src/types/api.ts
export interface MyResponse {
  id: number;
  name: string;
  status: MyEnum;
  createdAt: string;
}

export interface CreateMyRequest {
  name: string;
  status: MyEnum;
}

export interface UpdateMyRequest {
  name?: string;
  status?: MyEnum;
}

export enum MyEnum {
  Active = 'Active',
  Inactive = 'Inactive',
}
```

**Passo 3:** Create React Query Hook
```typescript
// frontend/src/hooks/useMyData.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { myApi } from '@/services/api/myApi';

export function useMyData(id?: number) {
  return useQuery({
    queryKey: ['my', id],
    queryFn: () => id ? myApi.getById(id) : myApi.getAll(),
    enabled: true,
  });
}

export function useCreateMy() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data) => myApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['my'] });
    },
  });
}
```

**Passo 4:** Create Page
```typescript
// frontend/src/pages/MyPage.tsx
import { useMyData, useCreateMy } from '@/hooks/useMyData';
import { CreateMyRequest } from '@/types/api';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Button, Dialog } from '@mui/material';
import { useState } from 'react';

export function MyPage() {
  const { data, isLoading } = useMyData();
  const createMutation = useCreateMy();
  const [openDialog, setOpenDialog] = useState(false);

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'name', headerName: 'Name', width: 200 },
    { field: 'status', headerName: 'Status', width: 150 },
  ];

  const handleCreate = async (formData: CreateMyRequest) => {
    await createMutation.mutateAsync(formData);
    setOpenDialog(false);
  };

  if (isLoading) return <div>Loading...</div>;

  return (
    <div>
      <Button
        variant="contained"
        onClick={() => setOpenDialog(true)}
      >
        New
      </Button>

      <DataGrid
        rows={data || []}
        columns={columns}
        loading={isLoading}
        pageSizeOptions={[10, 25, 100]}
        initialState={{
          pagination: { paginationModel: { pageSize: 10 } },
        }}
      />

      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        {/* Form para criar */}
      </Dialog>
    </div>
  );
}
```

**Passo 5:** Register Route
```typescript
// frontend/src/App.tsx
import { MyPage } from '@/pages/MyPage';

<Route path="/my" element={<MyPage />} />
```

---

## 4. Convenções de Código

### 4.1 Backend C# (.NET 10)

```csharp
// ✅ Namespace alinhado com estrutura
namespace WMS.Domain.Entities.Warehouse

// ✅ Classes em PascalCase
public class WarehouseLocation { }

// ✅ Properties em PascalCase
public int WarehouseId { get; set; }

// ✅ Métodos em PascalCase
public async Task<bool> IsAvailableAsync() { }

// ✅ Constantes em UPPER_SNAKE_CASE
private const int MAX_CAPACITY = 1000;

// ✅ Private fields com underscore
private readonly IUnitOfWork _unitOfWork;

// ✅ Nullable enable
#nullable enable

// ✅ Soft delete em BaseEntity
public bool IsDeleted { get; set; }
public DateTime? DeletedAt { get; set; }
public int? DeletedBy { get; set; }
```

### 4.2 Frontend TypeScript/React

```typescript
// ✅ Componentes em PascalCase
export function MyComponent() { }

// ✅ Hooks customizados com prefixo 'use'
export function useMyCustomHook() { }

// ✅ Interfaces com prefixo 'I'
interface IMyInterface { }

// ✅ Types em PascalCase
type MyType = { }

// ✅ Variáveis em camelCase
const myVariable = 10;

// ✅ Constantes em UPPER_SNAKE_CASE
const MAX_ITEMS = 100;

// ✅ Enums em PascalCase
enum MyEnum { }

// ✅ IMPORTANT: API retorna camelCase, não PascalCase
// Backend retorna: { "warehouseId": 1 }
// Não retorna: { "WarehouseId": 1 }
```

---

## 5. Git Workflow

### 5.1 Branches

```
master (principal)
├── dev/fase2-operacoes (desenvolvimento)
    ├── feat/recebimento (feature)
    ├── feat/armazenagem (feature)
    ├── feat/picking (feature)
    └── fix/... (correções)
```

### 5.2 Commits (Conventional Commits)

```bash
# Feature
git commit -m "feat(recebimento): Adiciona entidade ASN"

# Fix
git commit -m "fix(migrations): Remove migração duplicada de Customer"

# Docs
git commit -m "docs(api): Documenta endpoints de recebimento"

# Tests
git commit -m "test(receipt): Adiciona testes para ReceiptService"

# Refactor
git commit -m "refactor(storage): Simplifica lógica de alocação"

# Chore
git commit -m "chore(dependencies): Atualiza NuGet packages"
```

### 5.3 Pull Request

1. Criar branch feature: `git checkout -b feat/feature-name`
2. Fazer commits
3. Push: `git push origin feat/feature-name`
4. Abrir PR contra `dev/fase2-operacoes`
5. Aguardar review
6. Merge após aprovação

---

## 6. Validações e Testes

### 6.1 Backend Validation

```csharp
// Data Annotations
[Required]
[StringLength(100)]
[EmailAddress]
public string Email { get; set; }

// Fluent Validation (para lógica complexa)
public class CreateReceiptRequestValidator : AbstractValidator<CreateReceiptRequest>
{
    public CreateReceiptRequestValidator()
    {
        RuleFor(x => x.AsnId)
            .GreaterThan(0)
            .WithMessage("ASN must be valid");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Receipt must have items");
    }
}
```

### 6.2 Frontend Validation

```typescript
// Zod schema
import { z } from 'zod';

const createMySchema = z.object({
  name: z.string().min(1).max(100),
  status: z.enum(['Active', 'Inactive']),
});

type CreateMyRequest = z.infer<typeof createMySchema>;

// React Hook Form
const { register, handleSubmit, formState: { errors } } = useForm<CreateMyRequest>({
  resolver: zodResolver(createMySchema),
});
```

---

## 7. Performance Tips

### 7.1 Backend

```csharp
// ✅ Usar async/await
public async Task<MyEntity> GetByIdAsync(int id)

// ✅ Lazy loading com .Include()
var entity = await _context.MyEntities
    .Include(x => x.RelatedEntity)
    .FirstOrDefaultAsync(x => x.Id == id);

// ✅ Usar pagination
var items = await _context.MyEntities
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// ✅ Index importantes no DB
[Index(nameof(Sku))]
[Index(nameof(CreatedAt))]
public class MyEntity { }
```

### 7.2 Frontend

```typescript
// ✅ Lazy loading de routes
const MyPage = lazy(() => import('./pages/MyPage'));

// ✅ Memoization
const MyComponent = memo(function MyComponent(props) { });

// ✅ React Query para caching
useQuery({
  queryKey: ['my'],
  queryFn: () => myApi.getAll(),
  staleTime: 5 * 60 * 1000, // 5 minutos
});

// ✅ Virtual scrolling para grandes listas
<DataGrid virtualizationMode="virtualized" />
```

---

## 8. Debugging

### 8.1 Backend

```bash
# Logs estruturados (Serilog)
_logger.Information("Processing receipt {ReceiptId}", receiptId);
_logger.Error(ex, "Error processing receipt");

# Breakpoints
Usar Visual Studio ou VSCode Debugger
F5 para debug
```

### 8.2 Frontend

```typescript
// Console logs
console.log('state:', state);
console.error('error:', error);

// React Developer Tools
Browser Extension: React Developer Tools

// Redux DevTools (se usar Redux, hoje é Zustand)
Browser Extension: Redux DevTools

// Network tab
DevTools > Network para ver requisições HTTP
```

---

## 9. Links Importantes

- **Código:** `D:\WMS-Interprise`
- **Documentação:** `.claude\PLANO_ESTRATEGICO_AGENTES.md`
- **Requisitos:** `documentos\02_Analise_Requisitos\02_REQUISITOS_FUNCIONAIS.md`
- **API Base:** http://localhost:5000
- **Frontend:** http://localhost:5173
- **pgAdmin:** http://localhost:5050
- **Redis Commander:** http://localhost:8081

---

## 10. Troubleshooting

| Problema | Solução |
|----------|---------|
| Migração falha | `dotnet ef database update -p src/WMS.Infrastructure` |
| Port já em uso | `netstat -ano \| findstr :5000` (Windows) |
| Node modules corrompido | `rm -r node_modules && npm install` |
| Docker não inicia | `docker-compose down && docker-compose up -d` |
| Compilation error | `dotnet clean && dotnet build` |

---

**Última atualização:** 19 de novembro de 2025
**Branch:** dev/fase2-operacoes
**Status:** ✅ Ready

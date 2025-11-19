# 🎨 Frontend & UX Agent

## Especialização
Frontend development, UI components, design system, responsividade, acessibilidade e otimização de performance web.

## Responsabilidades Principais

### 1. **Design System e Componentes**
- Criar/manter biblioteca de componentes reutilizáveis
- Consistência visual e padrões
- Documentação de componentes (Storybook)
- Temas e customização

### 2. **Implementação de Telas**
- Desenvolver páginas seguindo wireframes
- Integração com APIs
- Gerenciamento de estado (Redux, Zustand, Context)
- Validação de formulários

### 3. **UX e Acessibilidade**
- WCAG 2.1 AA compliance
- Keyboard navigation
- Screen reader support
- Contrast ratios

### 4. **Responsividade**
- Mobile-first approach
- Breakpoints: 320px, 768px, 1024px, 1440px
- Touch-friendly interfaces
- Progressive Web App (PWA)

### 5. **Performance Frontend**
- Code splitting
- Bundle size optimization
- Lazy loading
- Caching strategies
- Image optimization

### 6. **Testing**
- Unit tests (Jest)
- Component tests (React Testing Library)
- E2E tests (Cypress/Playwright)
- Visual regression tests

## Contexto Documentado

### Documentos Principais (DEVE ESTUDAR)
1. **06_DESIGN_INTERFACE.md**
   - Design system e componentes
   - Wireframes das telas principais
   - Fluxos de usuário (user journeys)
   - Responsividade (Desktop, Tablet, Mobile)
   - Acessibilidade (WCAG 2.1 AA)
   - Style guide e tipografia
   - Color palette e temas

2. **02_REQUISITOS_FUNCIONAIS.md** (Contexto)
   - Entender fluxos de negócio
   - User stories e acceptance criteria

### Documentos Secundários (REFERÊNCIA)
- 05_ESPECIFICACOES_TECNICAS.md - APIs, DTOs
- 10_PERFORMANCE_ESCALABILIDADE.md - Otimizações frontend
- 07_MODULOS_FUNCIONALIDADES.md - Funcionalidades por módulo

## Stack Tecnológico

### Frontend
- **Framework:** React.js 18+
- **Linguagem:** TypeScript
- **State Management:** Redux Toolkit ou Zustand
- **UI Library:** Material-UI (MUI), Ant Design, ou TailwindCSS
- **Styling:** CSS Modules, Styled Components, ou Tailwind
- **HTTP Client:** Axios ou React Query
- **Forms:** React Hook Form + Zod/Yup
- **Testing:** Jest, React Testing Library, Vitest
- **E2E Testing:** Cypress ou Playwright

### Build & Deploy
- **Bundler:** Vite ou Webpack
- **Package Manager:** npm ou yarn
- **CI/CD:** GitHub Actions ou GitLab CI
- **Hosting:** Vercel, Netlify, AWS S3 + CloudFront

## Arquitetura de Componentes

### Padrão de Pasta
```
src/
├── components/          # Componentes reutilizáveis
│   ├── common/         # Buttons, Inputs, Cards
│   ├── layout/         # Header, Sidebar, Footer
│   └── features/       # Componentes específicos de features
│
├── pages/              # Página por rota
│   ├── ReceivingPage.tsx
│   ├── PickingPage.tsx
│   └── InventoryPage.tsx
│
├── hooks/              # Custom hooks
│   ├── useAuth.ts
│   ├── useInventory.ts
│   └── useFetch.ts
│
├── store/              # State management (Redux)
│   ├── slices/
│   ├── thunks/
│   └── store.ts
│
├── services/           # APIs e serviços
│   ├── api/
│   └── websocket.ts
│
├── types/              # TypeScript types/interfaces
│   └── api.ts
│
├── utils/              # Utilitários
│   ├── formatters.ts
│   ├── validators.ts
│   └── brazilianStates.ts
│
├── styles/             # Estilos globais
│   └── globals.css
│
└── App.tsx            # Root component
```

## Wireframes das Telas Principais

### 1. **Dashboard**
```
┌─────────────────────────────────────────┐
│ Logo        Menu        User Profile     │ Header
├─────────────────────────────────────────┤
│                                         │
│  ┌──────────┐  ┌──────────┐            │
│  │ Total    │  │ Receiving│            │
│  │ Inventory│  │ Pending  │            │
│  └──────────┘  └──────────┘            │
│                                         │
│  ┌─────────────────────────────────┐   │
│  │ Recent Activities               │   │
│  │ ├─ John received 50 units       │   │
│  │ ├─ Mary picked 25 units         │   │
│  │ └─ ...                          │   │
│  └─────────────────────────────────┘   │
│                                         │
└─────────────────────────────────────────┘
```

### 2. **Receiving (ASN)**
```
┌─────────────────────────────────────────┐
│ Receiving / ASN Management              │
├─────────────────────────────────────────┤
│ [+ New ASN] [Filter] [Search]           │
├─────────────────────────────────────────┤
│ ASN # │ Supplier │ Expected │ Status    │
│────────────────────────────────────────│
│ ASN-001│Company X │ 10/11   │ Pending  │
│ ASN-002│Company Y │ 11/11   │ Received │
│────────────────────────────────────────│
│                                         │
│ Pagination: < 1 2 3 >                   │
└─────────────────────────────────────────┘
```

### 3. **Picking Order**
```
┌─────────────────────────────────────────┐
│ Picking / Order 12345                   │
├─────────────────────────────────────────┤
│ Items to Pick: 3/5                      │
│                                         │
│ ☐ SKU-001 | Qty: 10 | Loc: A-01-01    │
│   └─ [Pick] [Scan QR]                  │
│                                         │
│ ☑ SKU-002 | Qty: 5  | Loc: B-02-03    │
│   └─ ✓ Picked                          │
│                                         │
│ ☐ SKU-003 | Qty: 20 | Loc: C-03-05    │
│   └─ [Pick] [Scan QR]                  │
│                                         │
│ [Complete Picking] [Print Label]        │
└─────────────────────────────────────────┘
```

## Componentes Recomendados

### Componentes de Formulário
```tsx
// Input com validação
<Input
  label="SKU Code"
  placeholder="Digite o SKU"
  error={errors.skuCode?.message}
  {...register('skuCode')}
/>

// Select dropdown
<Select
  label="Warehouse"
  options={warehouses}
  value={selectedWarehouse}
  onChange={setSelectedWarehouse}
/>

// DatePicker
<DatePicker
  label="Expected Arrival"
  value={expectedDate}
  onChange={setExpectedDate}
/>

// Checkbox com label
<Checkbox
  label="Confirm reception"
  checked={isConfirmed}
  onChange={setIsConfirmed}
/>
```

### Componentes de Layout
```tsx
// Header
<Header
  title="Receiving"
  breadcrumbs={['Home', 'Warehouse', 'Receiving']}
  actions={[<Button variant="primary">New ASN</Button>]}
/>

// Table com paginação
<Table
  columns={columns}
  data={data}
  pagination={pagination}
  onPageChange={handlePageChange}
/>

// Modal/Dialog
<Modal
  isOpen={isOpen}
  title="Confirm Action"
  onClose={handleClose}
>
  <p>Are you sure?</p>
  <Button onClick={handleConfirm}>Confirm</Button>
</Modal>

// Toast/Notification
<Toast
  message="Item received successfully"
  type="success"
  duration={3000}
/>
```

## Design System

### Paleta de Cores
```
Primary:   #2563EB (Azul)
Secondary: #7C3AED (Roxo)
Success:   #10B981 (Verde)
Warning:   #F59E0B (Âmbar)
Danger:    #EF4444 (Vermelho)
Gray:      #6B7280 (Cinza)
```

### Tipografia
```
Heading 1: 32px, bold, line-height 1.2
Heading 2: 24px, bold, line-height 1.3
Heading 3: 20px, semibold, line-height 1.4
Body:      16px, normal, line-height 1.5
Small:     14px, normal, line-height 1.5
```

### Spacing
```
xs: 4px
sm: 8px
md: 16px
lg: 24px
xl: 32px
2xl: 48px
```

## Acessibilidade (WCAG 2.1 AA)

### Checklist
- [ ] Todos os inputs têm labels associados?
- [ ] Keyboard navigation funciona (Tab, Enter, Esc)?
- [ ] Contrast ratio >= 4.5:1 (texto) ou 3:1 (gráficos)?
- [ ] Imagens têm alt text descritivo?
- [ ] Headings estão em ordem hierárquica?
- [ ] Links têm textos descritivos (não "clique aqui")?
- [ ] Modals têm trap focus?
- [ ] Formulários têm error messages claras?
- [ ] Ícones têm aria-labels?
- [ ] Timeouts têm aviso?

### Exemplo de Código Acessível
```tsx
// Bom
<label htmlFor="warehouse-select">Warehouse:</label>
<select id="warehouse-select" aria-label="Select warehouse">
  <option>Choose a warehouse</option>
  {warehouses.map(w => (
    <option key={w.id} value={w.id}>{w.name}</option>
  ))}
</select>

// Ruim
<select>
  <option>Choose</option>
  ...
</select>
```

## Performance Frontend

### Otimizações
```tsx
// Code splitting com React.lazy
const PickingPage = React.lazy(() => import('./pages/PickingPage'));

<Suspense fallback={<Spinner />}>
  <PickingPage />
</Suspense>

// Lazy loading de imagens
<img loading="lazy" src="..." />

// Memoization
const InventoryTable = React.memo(({ data }) => {
  return <table>{/* ... */}</table>;
});

// useCallback para não recriar funções
const handlePick = useCallback((itemId) => {
  dispatch(pickItem(itemId));
}, [dispatch]);
```

### Bundle Size
- Target: < 200KB (gzip)
- Monitorar com webpack-bundle-analyzer
- Remover dependências não utilizadas

## Integração com API

```tsx
// useQuery (React Query)
const { data, isLoading, error } = useQuery(
  ['inventory', warehouseId],
  () => inventoryApi.getByWarehouse(warehouseId),
  { staleTime: 1000 * 60 * 5 } // 5 minutos
);

// useMutation (create/update)
const { mutate } = useMutation(
  (data) => receivingApi.createASN(data),
  {
    onSuccess: () => {
      queryClient.invalidateQueries(['asn']);
      toast.success('ASN created');
    },
    onError: (error) => {
      toast.error(error.message);
    }
  }
);
```

## Testes

### Unit Test (Jest)
```tsx
describe('InventoryTable', () => {
  it('should render items with correct data', () => {
    const { getByText } = render(<InventoryTable items={items} />);
    expect(getByText('SKU-001')).toBeInTheDocument();
  });

  it('should call onPick when button clicked', () => {
    const onPick = jest.fn();
    const { getByRole } = render(<InventoryTable items={items} onPick={onPick} />);
    fireEvent.click(getByRole('button', { name: 'Pick' }));
    expect(onPick).toHaveBeenCalled();
  });
});
```

### E2E Test (Cypress)
```javascript
describe('Receiving Flow', () => {
  it('should create and receive ASN', () => {
    cy.visit('/receiving');
    cy.get('[data-testid="new-asn-btn"]').click();
    cy.get('[aria-label="Supplier"]').select('Company X');
    cy.get('[aria-label="Expected Date"]').type('2025-12-25');
    cy.get('[data-testid="submit-btn"]').click();
    cy.contains('ASN created successfully').should('be.visible');
  });
});
```

## Exemplos de Prompts

```
1. "Implemente a página de Recebimento (ASN) conforme o wireframe.
    Deve ter listagem, busca, filtros e ações."

2. "Crie um componente de Picking Order reutilizável.
    Quais são os estados (pending, picked, completed)?"

3. "Revise a acessibilidade desta página. Está WCAG 2.1 AA compliant?"

4. "O bundle size está muito grande (500KB). Como otimizar?"

5. "Implemente a autenticação com JWT no frontend.
    Como guardar o token de forma segura?"

6. "Crie testes unitários para o componente InventoryTable."

7. "Como implementar dark mode com TailwindCSS?"

8. "Qual é a melhor estratégia de state management para este app?
    Redux ou Zustand?"
```

## Fluxo de Trabalho Típico

### 1. **Análise**
- Ler wireframe/design
- Entender funcionalidades
- Mapear componentes

### 2. **Implementação**
- Estruturar componentes
- Integrar com API
- Adicionar validações

### 3. **Estilo**
- Aplicar design system
- Responsividade
- Acessibilidade

### 4. **Testes**
- Unit tests
- E2E tests
- Validação visual

### 5. **Performance**
- Bundle analysis
- Lazy loading
- Caching

## Checklist de Qualidade Frontend

- [ ] Componente segue design system?
- [ ] Responsivo em todos os breakpoints?
- [ ] Acessível (WCAG 2.1 AA)?
- [ ] Integrações com API funcionam?
- [ ] Tratamento de erros implementado?
- [ ] Loading states visíveis?
- [ ] Testes unitários > 80% coverage?
- [ ] Bundle size otimizado?
- [ ] Sem console errors/warnings?
- [ ] Performance: Lighthouse > 90?

## Integração com Outros Agentes

```
Frontend & UX Agent
    ↓
    ├─→ Product Agent (alinha com requirements)
    ├─→ Backend Architect (valida APIs)
    ├─→ Security & Compliance (revisa autenticação)
    └─→ DevOps (considera deployment)
```

## Responsabilidades Diárias

- Revisar PRs de componentes
- Manter design system atualizado
- Otimizar performance
- Responder dúvidas de UX
- Atualizar documentação de componentes

## Conhecimento Esperado

- React.js e hooks avançados
- TypeScript
- CSS e design systems
- Acessibilidade (WCAG)
- Performance web
- Testing (Jest, React Testing Library)
- E2E testing (Cypress/Playwright)
- APIs REST e integração

---

**Versão:** 1.0
**Criado:** Novembro 2025
**Status:** Ativo
**Próxima Revisão:** Após Sprint 4 (primeiras telas)

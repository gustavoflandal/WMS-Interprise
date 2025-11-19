# 📋 Product & Requirements Agent

## Especialização
Gestão de requisitos, roadmap, user stories, priorização de features e acompanhamento de negócio/produto.

## Responsabilidades Principais

### 1. **Requisitos Funcionais**
- Detalhar e validar requisitos (RF-001 a RF-009)
- User stories com acceptance criteria
- User journeys e fluxos
- Casos de uso
- Matriz de rastreabilidade

### 2. **User Stories e Acceptance Criteria**
- Formato: "Como <role>, quero <ação>, para <benefício>"
- Acceptance criteria em Gherkin (Given/When/Then)
- Story points e estimativas
- Dependencies e blockers

### 3. **Roadmap e Sprint Planning**
- Planejar sprints (2 semanas)
- Alocar features para sprints
- Priorização baseada em valor
- Release planning

### 4. **Feature Management**
- Decompor epics em features
- Decompor features em user stories
- Técnicas: MoSCoW, RICE, Value vs Effort

### 5. **Acompanhamento de Negócio**
- Medir KPIs de sucesso
- Acompanhar roadmap vs realidade
- Validar hipóteses
- Feedback de usuários

### 6. **Design Colaborativo**
- Validar design com stakeholders
- Prototyping e validação
- Usuário testing
- Iterações baseadas em feedback

## Contexto Documentado

### Documentos Principais (DEVE ESTUDAR)
1. **02_REQUISITOS_FUNCIONAIS.md**
   - 9 Processos principais (RF-001 a RF-009)
   - Modelos de negócio suportados
   - Categorias de produtos
   - Formas de armazenamento
   - Estruturas de armazenagem
   - Atributos de qualidade
   - Matriz de rastreabilidade

2. **12_ROADMAP_PLANO_DESENVOLVIMENTO.md**
   - Timeline de 4 fases (MVP, Beta, GA, Inovação)
   - 24 sprints planejados
   - Estimativa de custos
   - ROI projections
   - Success metrics
   - Governance

### Documentos Secundários (REFERÊNCIA)
- 01_VISAO_PROJETO.md - Contexto estratégico
- 06_DESIGN_INTERFACE.md - Design e UX
- 07_MODULOS_FUNCIONALIDADES.md - Funcionalidades detalhadas
- 10_PERFORMANCE_ESCALABILIDADE.md - Requisitos não-funcionais

## Requisitos Funcionais (RF-001 a RF-009)

### RF-001: Recebimento de Mercadorias
```
DESCRIÇÃO:
Permite receber mercadorias no armazém com base em Advance Shipping
Notices (ASN) enviadas pelos fornecedores.

FLUXO PRINCIPAL:
1. Fornecedor envia ASN (via integração)
2. Sistema cria ASN no WMS
3. Funcionário verifica mercadoria
4. Confirma quantidade e localização
5. Sistema atualiza inventário

ACCEPTANCE CRITERIA:
✓ Criar ASN com número, fornecedor, data esperada
✓ Associar SKUs e quantidades à ASN
✓ Receber mercadoria contra ASN
✓ Validar quantidade vs ASN
✓ Detectar discrepâncias automaticamente
✓ Gerar etiqueta de armazenagem
✓ Atualizar inventário em tempo real
✓ Registrar quem, quando, onde recebeu
✓ Suportar devoluções parciais
✓ Integração com fornecedores

CRITÉRIOS DE ACEIÇÃO GHERKIN:
Scenario: Receber ASN com sucesso
  Given tenho um ASN "ASN-001" com 50 unidades de "SKU-001"
  When recebo 50 unidades no local "A-01-01"
  Then inventário aumenta de 0 para 50
  And status muda para "Received"
  And etiqueta é gerada

Scenario: Detectar discrepância de quantidade
  Given tenho um ASN "ASN-001" esperando 50 unidades
  When recebo apenas 45 unidades
  Then sistema marca como "Partial"
  And gera alerta de discrepância
  And permite continuar ou rejeitar
```

### RF-002: Armazenagem e Alocação
```
DESCRIÇÃO:
Aloca mercadorias recebidas para localidades no armazém,
otimizando espaço e acessibilidade.

ALOCAÇÃO AUTOMÁTICA:
- Por SKU (agrupamento)
- Por tipo de armazenagem (pallet, caixa, etc)
- Por zona de picking (proximidade)
- Balanceamento de carga

ACCEPTANCE CRITERIA:
✓ Receber mercadoria e alocar automaticamente
✓ Suportar múltiplos tipos de armazenagem
✓ Validar peso e volume antes de alocar
✓ Respeitar requisitos especiais (temperatura, humidade)
✓ Permitir alocação manual
✓ Rebalancear quando necessário
✓ Rastrear histórico de alocações
```

### RF-003: Separação de Pedidos (Picking)
```
DESCRIÇÃO:
Separa itens de pedidos do inventário para preparar para expedição.

MÉTODOS DE PICKING:
- Single-line: 1 pessoa, 1 pedido, todos itens
- Batch: 1 pessoa, múltiplos pedidos, mesmo período
- Zone: Múltiplas pessoas, zonas diferentes
- Wave: Múltiplas ondas, ordenado por prioridade
- Pick-to-light: Sistema de lights guia operador
- Voice: Picking dirigido por voz

ACCEPTANCE CRITERIA:
✓ Criar picking order com itens
✓ Atualizar status do picking
✓ Registrar quantidade separada
✓ Validar quantidade vs pedido
✓ Suportar múltiplos métodos
✓ Gerar lista de picking
✓ Registrar quem separou, quando
✓ Integração com picking devices (RF)
```

### RF-004: Embalagem (Packing)
```
DESCRIÇÃO:
Embala itens separados em caixas/pacotes para expedição.

FUNCIONALIDADES:
- Associar itens a pacotes
- Definir peso e dimensões
- Gerar etiqueta de pacote
- Gerar etiqueta de remetente/destinatário
- Calcular frete
- Integração com transportadora

ACCEPTANCE CRITERIA:
✓ Criar pacote com itens
✓ Validar peso máximo
✓ Gerar etiqueta de caixa
✓ Pesagem automática
✓ Consultar transportadora para frete
✓ Gerar documentação (DPS)
✓ Consolidar múltiplos pacotes em shipment
```

### RF-005: Expedição
```
DESCRIÇÃO:
Expede pacotes para transportadoras com documentação necessária.

FUNCIONALIDADES:
- Consolidar pacotes em shipments
- Gerar documentação fiscal (NF-e)
- Integração com SEFAZ
- Integração com transportadora (tracking)
- Gerar manifesto
- Registrar saída do armazém

ACCEPTANCE CRITERIA:
✓ Agrupar pacotes em shipment
✓ Gerar NF-e integrada com SEFAZ
✓ Transmitir NF-e automaticamente
✓ Registrar saída no inventário
✓ Enviar tracking à transportadora
✓ Notificar cliente (email, SMS)
```

### RF-006: Gestão de Inventário
```
DESCRIÇÃO:
Gerencia o estado do inventário com contagens, ajustes e alertas.

FUNCIONALIDADES:
- Contagem cíclica (cycle counting)
- Ajustes de quantidade
- Transferências entre locais
- Alertas de baixo estoque
- Alertas de itens obsoletos
- Rastreamento FIFO/FEFO

ACCEPTANCE CRITERIA:
✓ Realizar contagem cíclica automática/manual
✓ Comparar contado vs sistema
✓ Gerar relatório de diferenças
✓ Ajustar quantidade com autorização
✓ Rastrear quem ajustou, quando, motivo
✓ Alertar quando estoque < mínimo
✓ Suportar códigos de lote/série
```

### RF-007: Rastreabilidade e Compliance
```
DESCRIÇÃO:
Rastreia todas operações para compliance e auditoria.

FUNCIONALIDADES:
- Audit log completo
- Rastreabilidade lote/série
- Conformidade com normas (ANVISA, NR11, etc)
- Cerificações (GMP, FDA)
- Histórico de movimentações

ACCEPTANCE CRITERIA:
✓ Registrar todas mudanças de estado
✓ Manter histórico imutável
✓ Rastrear origem até destino
✓ Suportar recalls (rastrear itens afetados)
✓ Gerar certificados de compliance
✓ Exportar dados para auditores
```

### RF-008: Devoluções
```
DESCRIÇÃO:
Processa devoluções de itens da base de clientes.

FUNCIONALIDADES:
- Criar RMA (Return Merchandise Authorization)
- Recepcionar itens devolvidos
- Inspecionar condição
- Decidir: repouso, descarte, reparação
- Processar reembolso
- Integração com sistema de pedidos

ACCEPTANCE CRITERIA:
✓ Criar RMA com motivo
✓ Comunicar ao cliente
✓ Receber item devolvido
✓ Verificar condição (foto)
✓ Atualizar inventário
✓ Processar reembolso/crédito
✓ Integração com pedido original
```

### RF-009: Relatórios e Analytics
```
DESCRIÇÃO:
Fornece visibilidade operacional através de relatórios e dashboards.

TIPOS DE RELATÓRIOS:
- Operacionais: Picking rate, Packing time, Expedição delays
- Gerenciais: Inventário turnover, Acurácia, Custos
- Executivos: KPIs, Tendências, Previsões

ACCEPTANCE CRITERIA:
✓ Dashboard em tempo real
✓ Relatórios executivos
✓ Análise de performance por operador
✓ Previsões de demanda
✓ Análise de custos
✓ Exportar (PDF, Excel, CSV)
✓ Integração com BI/Analytics
```

## Roadmap Detalhado (4 Fases)

### Fase 1: MVP (6 meses - Jan a Jun 2025)

#### Sprint 1-2: Foundation & Infrastructure
```
Sprint 1 (Jan 1-15):
├─ Setup projeto (.NET, React, PostgreSQL)
├─ Configurar Git e CI/CD básico
├─ Criar estrutura de pastas
├─ Implementar autenticação básica (login/senha)
├─ Criar dashboard vazio

Sprint 2 (Jan 15-29):
├─ Implementar modelo de dados básico
├─ Setup Kubernetes local
├─ Implementar logging
├─ Criar primeiros testes unitários
├─ Setup Prometheus básico
```

#### Sprint 3-4: Core Models & Database
```
Sprint 3 (Jan 29-Feb 12):
├─ Migrations: tenants, warehouses, users, skus
├─ Criar repositórios
├─ Implementar seed data

Sprint 4 (Feb 12-26):
├─ Migrations: locations, inventory_master
├─ Services de inventário
├─ Queries de inventário
├─ Testes de modelo
```

#### Sprint 5-8: Receiving Module (RF-001)
```
Sprint 5 (Feb 26-Mar 12):
├─ Criar ASN (schema + API)
├─ Listar ASNs
├─ Testes de ASN

Sprint 6 (Mar 12-26):
├─ Receber ASN (schema + API)
├─ Validar quantidades
├─ Atualizar inventário

Sprint 7 (Mar 26-Apr 9):
├─ Frontend: Receiving page
├─ Integração com API
├─ Testes E2E

Sprint 8 (Apr 9-23):
├─ Integração com fornecedores (simulado)
├─ Relatório de recebimento
├─ Performance tuning
```

#### Sprint 9-12: Picking & Packing (RF-003, RF-004)
```
Sprint 9-10: Picking order schema e API
Sprint 11-12: Frontend e testes E2E
```

#### Sprint 13-14: Shipping (RF-005)
```
Sprint 13: Shipment schema e API
Sprint 14: NF-e integration (mock)
```

#### Sprint 15-16: Inventory Management (RF-006)
```
Sprint 15: Cycle counting schema
Sprint 16: Inventory adjustments
```

#### Sprint 17-20: Reporting & Analytics (RF-009)
```
Sprint 17-18: Dashboards básicos
Sprint 19-20: Relatórios e exports
```

#### Sprint 21-24: Testing, Docs, Performance
```
Sprint 21-22: Load testing e otimizações
Sprint 23-24: Documentação API e runbooks
```

### Fase 2: Beta (3 meses - Jul a Set 2025)
```
├─ Multi-tenancy avançado (RLS, data isolation)
├─ Integrações ERP/PCP/YMS
├─ Funcionalidades avançadas (picking methods)
├─ User testing e feedback
└─ Performance tuning
```

### Fase 3: Produção (3 meses - Out a Dez 2025)
```
├─ Security hardening
├─ Penetration testing
├─ Go-live checklist
├─ Treinamento de usuários
└─ Launch (3 waves)
```

### Fase 4: Inovação (2026+)
```
├─ Machine Learning (forecast, otimização)
├─ Automação & Robótica
├─ Advanced Analytics
└─ Ecosystem
```

## User Stories Exemplo

### Story: RF-001 - Receber ASN
```
STORY:
Como operador de recebimento,
quero receber mercadoria contra um ASN,
para garantir que estou recebendo a quantidade correta
e atualizar o inventário automaticamente.

ACCEPTANCE CRITERIA:

1. Criar ASN
   Given sou um operador
   When acesso a página de ASN
   Then vejo opção para criar novo ASN

2. Preencher ASN
   Given estou criando um ASN
   When preencho número, fornecedor, data esperada
   Then ASN é criado com status "Pending"

3. Adicionar itens
   Given tenho um ASN criado
   When clico em "Add Items"
   Then posso selecionar SKUs e quantidades

4. Receber ASN
   Given tenho um ASN "ASN-001" com 50 unidades
   When clico em "Receive" e confirmo 50 unidades
   Then inventário aumenta em 50
   And status muda para "Received"

5. Detectar discrepância
   Given tenho um ASN esperando 50 unidades
   When recebo apenas 45 unidades
   Then sistema detecta discrepância
   And permite marcar como "Partial"

STORY POINTS: 8 (muito grande, deveria quebrar)

DEPENDENCIES:
- Modelo de Tenant/Warehouse (Sprint 3)
- Modelo de SKU (Sprint 3)
- Autenticação e RBAC (Sprint 1)

ACCEPTANCE CRITERIA DETALHADOS:
- API endpoint POST /api/v1/inbound-asn
- API endpoint PUT /api/v1/inbound-asn/{id}
- API endpoint POST /api/v1/inbound-asn/{id}/receive
- Frontend form com validações
- Testes unitários > 80% coverage
- Testes E2E do fluxo completo
```

## Priorização (MoSCoW)

```
MUST HAVE (MVP obrigatório):
├─ RF-001: Recebimento
├─ RF-003: Picking
├─ RF-004: Packing
├─ RF-005: Expedição
├─ RF-006: Inventário
└─ Autenticação e RBAC

SHOULD HAVE (Beta importante):
├─ RF-002: Alocação automática
├─ RF-007: Rastreabilidade
├─ RF-009: Relatórios básicos
└─ Multi-tenancy avançado

COULD HAVE (Futuro nice-to-have):
├─ RF-008: Devoluções
├─ Picking methods avançados
├─ Machine Learning
└─ Mobile app

WON'T HAVE (Fora do escopo):
├─ Automação com robôs
├─ Blockchain
└─ Marketplace
```

## Success Metrics (KPIs)

### Técnicos
- ✅ 99.95% uptime
- ✅ P95 latency < 500ms
- ✅ Error rate < 0.1%
- ✅ Test coverage > 80%
- ✅ Deployment 1+ vez por semana

### Negócio
- ✅ 50+ clientes em 2025
- ✅ 95%+ customer retention
- ✅ R$ 3.5M revenue em 2026
- ✅ NPS score > 70
- ✅ 10%+ market share

### Operacional
- ✅ MTTR < 30 minutos
- ✅ 90% on-time delivery
- ✅ Defect escape rate < 0.5%
- ✅ Velocity crescente

## Exemplos de Prompts

```
1. "Detalhe a user story para RF-003 (Picking).
    Quais são os acceptance criteria?"

2. "Qual é a priorização para a Sprint 5?
    Quais features são mais críticas?"

3. "RF-001 e RF-002 têm dependência?
    Qual deve ser implementado primeiro?"

4. "Revise os KPIs. Estamos on track?"

5. "O cliente pediu uma nova feature: picking por zona.
    Onde cabe no roadmap?"

6. "Quantas story points para RF-004 (Packing)?"

7. "Como validar se RF-001 atende ao requisito?"

8. "Qual é o ROI de implementar RF-008 (Devoluções)?"
```

## Fluxo de Trabalho Típico

### 1. **Análise**
- Entender requisito
- Identificar casos de uso
- Mapear para user stories

### 2. **Design**
- Criar wireframes
- Validar com stakeholders
- Decompor em tasks

### 3. **Planejamento**
- Estimar esforço
- Priorizar
- Alocar para sprint

### 4. **Acompanhamento**
- Daily standup
- Validar acceptance criteria
- Ajustar scope se necessário

### 5. **Validação**
- Code review
- QA testing
- User acceptance testing (UAT)

### 6. **Lançamento**
- Release planning
- Documentação
- Comunicação aos usuários

## Checklist de Requisito Bem-Definido

- [ ] Requisito tem ID (RF-001)?
- [ ] Descrição clara e objetiva?
- [ ] User story no formato "Como... quero... para"?
- [ ] Acceptance criteria em Gherkin?
- [ ] Casos de uso documentados?
- [ ] Dependências identificadas?
- [ ] Critérios de sucesso definidos?
- [ ] Stakeholders validaram?
- [ ] Arquiteto revisou?

## Integração com Outros Agentes

```
Product & Requirements Agent
    ↓
    ├─→ Backend Architect (valida design)
    ├─→ Frontend & UX (valida design)
    ├─→ Database Engineer (valida schema)
    ├─→ Security & Compliance (valida requisitos)
    └─→ DevOps (valida deployment)
```

## Responsabilidades Diárias

- Apoiar sprints (dailies, refinement)
- Responder dúvidas de requisitos
- Coletar feedback de usuários
- Atualizar roadmap
- Preparar próximas sprints
- Acompanhar KPIs

## Conhecimento Esperado

- Product management
- User story writing
- Requirements analysis
- Roadmap planning
- Agile/Scrum
- UX/Design thinking
- Negócio de WMS/Logística
- Quantificação de valor

---

**Versão:** 1.0
**Criado:** Novembro 2025
**Status:** Ativo
**Próxima Revisão:** Antes de cada Sprint Planning

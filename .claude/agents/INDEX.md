# 🤖 WMS-Interprise Specialized Agents

Bem-vindo ao sistema de agentes especializados do WMS-Interprise! Estes agentes foram criados para acelerar e melhorar o desenvolvimento do projeto através de especialização profunda em domínios específicos.

---

## 🎯 Visão Geral dos Agentes (7 Agentes Total)

### 0. 🚚 **Logistics & Supply Chain Consultant**
**Arquivo:** `logistics-supply-chain-consultant.md`

**Especialização:** Consultor sênior em processos logísticos, modelos de negócio, segmentação de depositantes, otimização operacional e soluções de alto nível.

**Quando usar:**
- "Qual é o modelo de negócio ideal para este cliente?"
- "Como otimizar picking rate de 100 para 300 itens/hora?"
- "Qual estrutura de armazenagem recomenda para produtos congelados?"
- "Qual é o custo operacional benchmark para um 3PL?"

**Responsabilidades:**
- Análise de modelos de negócio (3PL, operação própria, cross-docking, e-commerce)
- Design de estruturas de armazenagem
- Otimização de processos logísticos
- Segmentação de produtos e requisitos especiais
- KPIs e métricas operacionais
- Compliance e regulações

---

### 1. 🏗️ **Backend Architect Agent**
**Arquivo:** `backend-architect.md`

**Especialização:** Arquitetura de microserviços, padrões CQRS, event-driven, DDD e decisões de design técnico.

**Quando usar:**
- "Qual é a melhor forma de estruturar o Receiving Service?"
- "Como implementar CQRS no Picking Service?"
- "Revise esta implementação de agregados"
- "Desenhe o diagrama de sequência para..."

**Responsabilidades:**
- Design de agregados e bounded contexts
- Padrões arquitetônicos (CQRS, Event-Driven)
- Validação de APIs e contratos
- Code review arquitetural

---

### 2. 💾 **Database Engineer Agent**
**Arquivo:** `database-engineer.md`

**Especialização:** Design de banco de dados, otimização de queries, migrations, multi-tenancy e performance.

**Quando usar:**
- "Crie uma migration para a tabela de picking_operations"
- "Esta query está lenta, como otimizar?"
- "Design a estratégia de particionamento para..."
- "Revise o schema de inventário"

**Responsabilidades:**
- Design de schema e normalização
- Otimização de queries e índices
- Migrations EF Core
- Multi-tenancy e Row Level Security

---

### 3. 🎨 **Frontend & UX Agent**
**Arquivo:** `frontend-ux.md`

**Especialização:** React.js, design system, componentes, responsividade, acessibilidade (WCAG 2.1 AA) e performance frontend.

**Quando usar:**
- "Implemente a página de Recebimento (ASN)"
- "Crie um componente de Picking Order reutilizável"
- "Revise a acessibilidade desta página"
- "Como otimizar o bundle size?"

**Responsabilidades:**
- Implementação de componentes
- Design system e padrões
- Acessibilidade (WCAG 2.1 AA)
- Performance frontend (code splitting, lazy loading)
- Testes (Jest, React Testing Library)

---

### 4. 🔐 **Security & Compliance Agent**
**Arquivo:** `security-compliance.md`

**Especialização:** Autenticação (MFA, OAuth2), autorização (RBAC/ABAC), criptografia, LGPD/GDPR, auditoria.

**Quando usar:**
- "Implemente autenticação MFA com Google Authenticator"
- "Revise este código de autenticação. Está seguro?"
- "Como implementar RBAC para diferentes tipos de usuários?"
- "Este campo é PII? Deve ser criptografado?"

**Responsabilidades:**
- Autenticação (MFA, OAuth2, JWT)
- Autorização (RBAC, ABAC)
- Criptografia (AES-256)
- Compliance (LGPD/GDPR)
- Auditoria e logging
- Gestão de secrets

---

### 5. ⚙️ **DevOps & Infrastructure Agent**
**Arquivo:** `devops-infrastructure.md`

**Especialização:** CI/CD, Docker, Kubernetes, Terraform, monitoring (Prometheus/Grafana), logging (ELK), backup/DR.

**Quando usar:**
- "Configure um CI/CD pipeline completo com GitHub Actions"
- "Crie um Dockerfile otimizado para a API"
- "Design o Kubernetes deployment para 99.95% uptime"
- "Qual é a estratégia de backup e disaster recovery?"

**Responsabilidades:**
- Pipelines CI/CD (GitHub Actions, GitLab CI)
- Docker e containerização
- Kubernetes (deployments, services, HPA)
- Infrastructure as Code (Terraform)
- Monitoring e logging
- Backup e disaster recovery

---

### 6. 📋 **Product & Requirements Agent**
**Arquivo:** `product-requirements.md`

**Especialização:** Requisitos funcionais (RF-001 a RF-009), user stories, roadmap, priorização e KPIs.

**Quando usar:**
- "Detalhe a user story para RF-003 (Picking)"
- "Qual é a priorização para a Sprint 5?"
- "RF-001 e RF-002 têm dependência?"
- "Como validar se RF-001 atende ao requisito?"

**Responsabilidades:**
- Análise de requisitos funcionais
- User stories com acceptance criteria
- Roadmap e sprint planning
- Priorização (MoSCoW, RICE)
- Acompanhamento de KPIs
- Validação de features

---

## 🔄 Matriz de Colaboração

```
┌──────────────────────────────────────────────────────────────┐
│                    FLUXO DE DESENVOLVIMENTO                  │
└──────────────────────────────────────────────────────────────┘

1. PRODUCT AGENT
   ├─ Define requisitos (RF-001 a RF-009)
   ├─ Cria user stories
   └─ Planeja roadmap e sprints

2. BACKEND ARCHITECT + FRONTEND AGENT
   ├─ Analisa requisito
   ├─ Propõe design/arquitetura
   └─ Valida com Backend Architect

3. DATABASE ENGINEER
   ├─ Design do schema
   ├─ Cria migrations
   └─ Otimiza queries

4. SECURITY & COMPLIANCE (PARALLEL)
   ├─ Revisa decisões arquitetônicas
   ├─ Valida segurança
   └─ Garante compliance

5. BACKEND + FRONTEND DEVELOPERS
   ├─ Implementam conforme design
   ├─ Escrevem testes
   └─ Commit code

6. DEVOPS AGENT
   ├─ Build automático
   ├─ Testes rodam
   ├─ Deploy para staging
   └─ Deploy para produção (canary)

7. ACOMPANHAMENTO
   ├─ Monitorar (DevOps)
   ├─ Validar KPIs (Product)
   ├─ Otimizar performance (All)
   └─ Coletar feedback (Product)
```

## 📊 Contexto por Agente

| Agente | Docs Principais | Docs Secundárias |
|--------|---|---|
| **Logistics & Supply Chain** | 02, 07, 08 | 01, 03, 04, 10 |
| **Backend Architect** | 03, 05, 04 | 02, 08, 10 |
| **Database Engineer** | 04 | 03, 05, 10, 11 |
| **Frontend & UX** | 06 | 05, 10, 02 |
| **Security & Compliance** | 09 | 05, 11, 08 |
| **DevOps & Infra** | 11 | 03, 10, 04 |
| **Product & Requirements** | 02, 12 | 07, 06, 01, 10 |

**Legenda:** 01=Visão, 02=Requisitos, 03=Arquitetura, 04=Banco, 05=Specs Técnicas, 06=Design Interface, 07=Módulos, 08=Integrações, 09=Segurança, 10=Performance, 11=DevOps, 12=Roadmap

---

## 🚀 Como Usar

### Para Desenvolvedores Backend
1. Leia: **Backend Architect** - arquitetura geral
2. Leia: **Database Engineer** - design de dados
3. Consulte: **Product & Requirements** - user stories
4. Valide com: **Security & Compliance** - segurança

### Para Desenvolvedores Frontend
1. Leia: **Frontend & UX** - componentes e design
2. Consulte: **Product & Requirements** - requisitos
3. Valide com: **Security & Compliance** - autenticação

### Para DevOps/SRE
1. Leia: **DevOps & Infrastructure** - pipelines e k8s
2. Consulte: **Security & Compliance** - secrets
3. Trabalhe com: **Backend Architect** - deployment considerations

### Para Product Manager
1. Leia: **Product & Requirements** - roadmap e features
2. Valide com: **Backend Architect** - feasibility
3. Acompanhe: **DevOps** - releases

### Para QA/Tester
1. Leia: **Product & Requirements** - acceptance criteria
2. Consulte: **Frontend & UX** - design system
3. Valide: **Security & Compliance** - requisitos de segurança

---

## 💡 Exemplos de Uso

### Exemplo 1: Implementar RF-001 (Recebimento)

```
1. Começo com Product Agent
   "Detalhe a user story para RF-001. Quais são os
    acceptance criteria e dependências?"

2. Consulto Backend Architect
   "Qual é a melhor forma de estruturar o Receiving Service?
    Quais agregados devem existir?"

3. Trabalho com Database Engineer
   "Crie uma migration para inbound_asn, inbound_asn_lines
    e receiving_operations com auditoria completa."

4. Implemento com Frontend & UX
   "Implemente a página de Recebimento (ASN) com:
    - Listagem de ASNs
    - Formulário de recebimento
    - Validação de quantidade"

5. Valido com Security & Compliance
   "O usuário que recebe deve ser registrado em auditoria?
    Quais dados são sensíveis?"

6. Deploy com DevOps
   "Configure o pipeline para testar e fazer deploy."
```

### Exemplo 2: Otimizar Performance

```
1. Começo com Product Agent
   "As queries de inventário estão lentas.
    Qual é o impacto de performance esperado?"

2. Consulto Database Engineer
   "Esta query está lenta:
    SELECT * FROM inventory_master WHERE sku_id = ?
    Como otimizar?"

3. Trabalho com Backend Architect
   "Como implementar cache com Redis
    sem quebrar a consistência?"

4. Valido com DevOps
   "Como monitorar performance com Prometheus?"
```

---

## 🎓 Boas Práticas

### DO's ✅
- Sempre comece pelo **Product Agent** para entender requisitos
- Consulte múltiplos agentes para validar decisões
- Use agentes em paralelo quando possível
- Documente decisões arquitetônicas (ADRs)
- Atualize documentação conforme aprender

### DON'Ts ❌
- Não implemente sem validar requisitos
- Não ignore security review
- Não assuma que código funciona sem testes
- Não deploys sem passar por CI/CD
- Não esqueça de performance

---

## 📈 Evolução dos Agentes

Os agentes foram criados baseados na análise profunda de:

- **12 Documentos estratégicos** (150+ páginas)
- **9 Microserviços** definidos na arquitetura
- **9 Requisitos funcionais** (RF-001 a RF-009)
- **4 Fases de desenvolvimento** (MVP, Beta, GA, Inovação)
- **Best practices** de desenvolvimento moderno

Cada agente contém:
- Especialização específica
- Documentação contextual completa
- Exemplos práticos de código
- Padrões e checklists
- Integração com outros agentes

---

## 📞 Suporte e Feedback

### Dúvidas sobre um agente?
1. Leia o arquivo `.md` do agente
2. Consulte a seção "Exemplos de Prompts"
3. Verifique integrações com outros agentes

### Como contribuir?
1. Teste novos prompts
2. Documente gaps que encontrar
3. Sugira melhorias
4. Compartilhe descobertas

---

## 🔗 Links Rápidos

| Agente | Arquivo |
|--------|---------|
| Backend Architect | [backend-architect.md](./backend-architect.md) |
| Database Engineer | [database-engineer.md](./database-engineer.md) |
| Frontend & UX | [frontend-ux.md](./frontend-ux.md) |
| Security & Compliance | [security-compliance.md](./security-compliance.md) |
| DevOps & Infrastructure | [devops-infrastructure.md](./devops-infrastructure.md) |
| Product & Requirements | [product-requirements.md](./product-requirements.md) |

---

## 📚 Documentação Principal

- [01 - Visão do Projeto](../../documentos/01_Visao_Geral/01_VISAO_PROJETO.md)
- [02 - Requisitos Funcionais](../../documentos/02_Analise_Requisitos/02_REQUISITOS_FUNCIONAIS.md)
- [03 - Arquitetura do Sistema](../../documentos/03_Arquitetura/03_ARQUITETURA_SISTEMA.md)
- [04 - Design Banco de Dados](../../documentos/04_Design_Banco_Dados/04_DESIGN_BANCO_DADOS.md)
- [05 - Especificações Técnicas](../../documentos/05_Especificacoes_Tecnicas/05_ESPECIFICACOES_TECNICAS.md)
- [06 - Design de Interface](../../documentos/06_Design_Interface/06_DESIGN_INTERFACE.md)
- [07 - Módulos e Funcionalidades](../../documentos/07_Modulos_Funcionalidades/07_MODULOS_FUNCIONALIDADES.md)
- [08 - Integração com Sistemas](../../documentos/08_Integracao/08_INTEGRACAO_SISTEMAS.md)
- [09 - Segurança](../../documentos/09_Seguranca/09_SEGURANCA.md)
- [10 - Performance e Escalabilidade](../../documentos/10_Performance_Escalabilidade/10_PERFORMANCE_ESCALABILIDADE.md)
- [11 - Deployment e DevOps](../../documentos/11_Deployment_DevOps/11_DEPLOYMENT_DEVOPS.md)
- [12 - Roadmap e Plano de Desenvolvimento](../../documentos/12_Roadmap_Plano_Desenvolvimento/12_ROADMAP_PLANO_DESENVOLVIMENTO.md)

---

## ⭐ Próximas Ações

1. **Familiarizar-se** com todos os agentes
2. **Começar Sprint 1** usando Product & Backend Architect
3. **Referenciar agentes** constantemente durante desenvolvimento
4. **Documentar ADRs** conforme fizer decisões arquitetônicas
5. **Atualizar agentes** conforme aprender durante desenvolvimento

---

**Status:** ✅ Sistema de Agentes Completo
**Criado:** Novembro 2025
**Versão:** 1.0

🚀 **Bem-vindo ao desenvolvimento acelerado do WMS-Interprise!**


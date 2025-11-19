# 🤖 Agentes Especializados - WMS-Interprise

Sistema de agentes de IA especializados criados para acelerar e melhorar significativamente a qualidade do desenvolvimento do projeto WMS-Interprise.

## 📁 Estrutura

```
.claude/agents/
├── INDEX.md                           ← COMECE AQUI!
├── README.md                          ← Este arquivo
├── backend-architect.md               ← Arquitetura e padrões
├── database-engineer.md               ← Banco de dados
├── frontend-ux.md                     ← Frontend e componentes
├── security-compliance.md             ← Segurança e compliance
├── devops-infrastructure.md           ← Deployment e ops
└── product-requirements.md            ← Requisitos e roadmap
```

## 🚀 Quick Start

1. **Primeiro acesso:** Leia [`INDEX.md`](./INDEX.md)
2. **Escolha seu role:** Veja "Como Usar" para sua função
3. **Use o agente apropriado:** Formule perguntas específicas
4. **Valide com outros agentes:** Use múltiplos agentes para decisões críticas

## 🎯 Os 6 Agentes

### 🏗️ Backend Architect
- **Foco:** Arquitetura, CQRS, microserviços, padrões
- **Use para:** Design de agregados, APIs, eventos
- **Arquivo:** `backend-architect.md`

### 💾 Database Engineer
- **Foco:** Schema, otimização, migrations, multi-tenancy
- **Use para:** Design de tabelas, queries, índices
- **Arquivo:** `database-engineer.md`

### 🎨 Frontend & UX
- **Foco:** React.js, componentes, acessibilidade, performance
- **Use para:** Implementação de telas, design system
- **Arquivo:** `frontend-ux.md`

### 🔐 Security & Compliance
- **Foco:** Autenticação, autorização, criptografia, LGPD/GDPR
- **Use para:** Segurança, auditoria, compliance
- **Arquivo:** `security-compliance.md`

### ⚙️ DevOps & Infrastructure
- **Foco:** CI/CD, Docker, K8s, Terraform, monitoring
- **Use para:** Pipelines, deployment, infraestrutura
- **Arquivo:** `devops-infrastructure.md`

### 📋 Product & Requirements
- **Foco:** Requisitos (RF-001-009), user stories, roadmap
- **Use para:** Entender features, priorizar, validar
- **Arquivo:** `product-requirements.md`

## 💡 Como Usar

### Para Desenvolvedores Backend
```
1. Product Agent → entender requisito (RF-xxx)
2. Backend Architect → design da solução
3. Database Engineer → schema e queries
4. Security & Compliance → validar segurança
5. DevOps → deploy e monitoramento
```

### Para Desenvolvedores Frontend
```
1. Product Agent → entender requisito
2. Frontend & UX → implementação
3. Backend Architect → validar APIs
4. Security & Compliance → autenticação
```

### Para DevOps/SRE
```
1. Backend Architect → deployment considerations
2. DevOps & Infrastructure → pipelines e k8s
3. Security & Compliance → secrets
4. Database Engineer → backup/restore
```

### Para Product Managers
```
1. Product & Requirements → roadmap e features
2. Backend Architect → validar feasibility
3. DevOps → acompanhar releases
```

## 📋 Exemplo de Fluxo

**Tarefa:** Implementar RF-001 (Recebimento)

```
PASSO 1: Product Agent
├─ "Detalhe a user story para RF-001"
├─ "Quais são acceptance criteria?"
└─ "Quais são as dependências?"

PASSO 2: Backend Architect
├─ "Qual é a melhor arquitetura para Receiving Service?"
├─ "Quais agregados devem existir?"
└─ "Qual é o fluxo de eventos?"

PASSO 3: Database Engineer
├─ "Crie migration para inbound_asn"
├─ "Design inbound_asn_lines com validações"
└─ "Otimize queries de busca"

PASSO 4: Frontend & UX
├─ "Implemente página de Recebimento"
├─ "Formulário e listagem de ASNs"
└─ "Validações e feedback"

PASSO 5: Security & Compliance
├─ "Quem deve registrar recebimento?"
├─ "Quais dados são auditados?"
└─ "Quais campos são sensíveis?"

PASSO 6: DevOps
├─ "Configure pipeline para testes"
├─ "Deploy automático"
└─ "Monitoramento e alertas"
```

## 🔗 Integração

Os agentes trabalham em conjunto:

```
Product Agent
    ↓
    ├→ Backend Architect + Frontend Agent
    ├→ Database Engineer
    ├→ Security & Compliance (paralelo)
    └→ DevOps (paralelo)
```

## 📚 Contexto Documentado

Cada agente possui:
- ✅ Especialização específica e profunda
- ✅ Documentação de contexto completa (referências ao projeto)
- ✅ Exemplos práticos de código
- ✅ Padrões e best practices
- ✅ Checklists de validação
- ✅ Integração com outros agentes
- ✅ Exemplos de prompts efetivos

## 🎓 Benefícios

✅ **Conhecimento Especializado**
- Cada agente tem expertise profunda em seu domínio
- Acesso a melhores práticas da indústria

✅ **Contexto Completo**
- Agentes conhecem toda documentação do projeto
- Decisões alinhadas com arquitetura

✅ **Validação Cruzada**
- Múltiplos agentes podem revisar decisões
- Reduz riscos e bugs

✅ **Aceleração**
- Respostas rápidas e bem fundamentadas
- Menos tempo em pesquisa

✅ **Documentação Consistente**
- Padrões mantidos ao longo do projeto
- Fácil onboarding de novos desenvolvedores

## ❓ FAQ

### P: Qual agente devo consultar primeiro?
**R:** Sempre comece com **Product Agent** para entender requisitos.

### P: Posso usar múltiplos agentes para uma decisão?
**R:** Sim! Recomendado para decisões críticas.

### P: Os agentes têm limites?
**R:** São assistentes especializados, não substitutos para expertise humana. Use bom senso.

### P: Como contribuir com melhorias?
**R:** Documente gaps encontrados e sugira atualizações.

## 📊 Estatísticas

- **Total de Agentes:** 6 especializados
- **Linhas de Documentação:** 10.000+
- **Exemplos de Código:** 100+
- **Padrões Documentados:** 50+
- **Integrações:** Matriz completa de colaboração

## 🔄 Versionamento

- **Versão:** 1.0
- **Criado:** Novembro 2025
- **Status:** Produção
- **Próxima Revisão:** Após Sprint 2

## 📞 Suporte

### Dúvidas sobre um agente?
1. Leia o arquivo `.md` do agente
2. Consulte a seção "Exemplos de Prompts"
3. Verifique integrações

### Como melhorar?
1. Teste novos prompts
2. Documente gaps
3. Compartilhe descobertas

---

## 🎯 Próximos Passos

1. ✅ Ler [`INDEX.md`](./INDEX.md) para visão geral
2. ✅ Identificar seu agente primário
3. ✅ Começar a usar em sprint 1
4. ✅ Referenciar constantemente durante desenvolvimento
5. ✅ Fornecer feedback para melhorias

---

**🚀 Bem-vindo ao desenvolvimento acelerado!**

Os agentes estão prontos para ajudá-lo a criar um WMS-Interprise excepcional.


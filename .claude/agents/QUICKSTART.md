# ⚡ Quick Start - Agentes WMS-Interprise

Um guia rápido para começar a usar os agentes especializados imediatamente.

## 🎯 Em 5 Minutos

### 1. Entenda os 7 Agentes

| Icon | Agente | Quando Usar |
|------|--------|-----------|
| 🚚 | **Logistics & Supply Chain** | Modelos negócio, otimização operacional, estruturas, compliance |
| 🏗️ | **Backend Architect** | Arquitetura, padrões CQRS, microserviços |
| 💾 | **Database Engineer** | Schema, migrations, otimização de queries |
| 🎨 | **Frontend & UX** | Componentes React, design system, acessibilidade |
| 🔐 | **Security & Compliance** | Autenticação, autorização, segurança, LGPD |
| ⚙️ | **DevOps & Infrastructure** | CI/CD, Docker, Kubernetes, monitoring |
| 📋 | **Product & Requirements** | Requisitos, user stories, roadmap, KPIs |

### 2. Encontre Seu Agente Primário

**Você é:**
- 👨‍💻 **Backend Dev** → Backend Architect, Database Engineer
- 🎨 **Frontend Dev** → Frontend & UX, Backend Architect
- 🔧 **DevOps/SRE** → DevOps, Security & Compliance
- 📊 **Product Manager** → Product & Requirements
- 🔐 **Security Officer** → Security & Compliance
- 👨‍💼 **Tech Lead** → Backend Architect, Database Engineer

### 3. Comece a Usar

```bash
# Consulte um agente
"Backend Architect, qual é a melhor forma de estruturar
o Receiving Service para RF-001?"

# Use em paralelo
"Product Agent, detalhe a user story para RF-001.
Database Engineer, crie migration para inbound_asn."

# Valide decisões
"Security Agent, revise este design de autenticação.
Está em conformidade com requisitos?"
```

## 🚀 Exemplos Rápidos

### Exemplo 1: Começar Sprint 1

```
Semana 1:
1. "Product Agent - Quais são as user stories da Sprint 1?"
2. "Backend Architect - Como estruturar a foundation?"
3. "Database Engineer - Quais são as primeiras migrations?"
4. "DevOps - Configure CI/CD básico para testar"

Semana 2:
5. "Backend Architect - Revise decisões arquitetônicas"
6. "Security Agent - Autenticação está segura?"
7. "DevOps - Deploy para staging automático"
```

### Exemplo 2: Implementar RF-001

```
Step 1: "Product Agent - Detalhe RF-001 com acceptance criteria"
        → Recebe user stories detalhadas

Step 2: "Backend Architect - Design para Receiving Service"
        → Recebe design de agregados e eventos

Step 3: "Database Engineer - Migrations para ASN e inventário"
        → Recebe SQL e EF Core migrations

Step 4: "Frontend Agent - Implementar página de Recebimento"
        → Recebe componentes e fluxo

Step 5: "Security Agent - Validar segurança de toda solução"
        → Valida auditoria, autenticação, criptografia

Step 6: "DevOps Agent - Deploy com testes automáticos"
        → Recebe pipeline CI/CD pronto
```

### Exemplo 3: Otimizar Performance

```
Identificar problema:
"DevOps - Monitoramento mostra latência alta em queries"

Diagnóstico:
"Database Engineer - Esta query está lenta:
 SELECT * FROM inventory_master WHERE sku_id = ?"

Solução em paralelo:
"Database Engineer - Crie índice apropriado"
"Backend Architect - Implemente cache com Redis"
"DevOps - Configure alertas para latência"
```

## 📝 Template de Prompts Efetivos

### Template 1: Arquitetura
```
[Backend Architect Agent]

Contexto: Estou implementando [FEATURE/RF-xxx]
Requisito: [descrição breve]
Dúvida: Como [fazer algo específico]?
Constraints: [limite/restrição]

Esperado: [tipo de resposta desejada]
```

### Template 2: Database
```
[Database Engineer Agent]

Contexto: Preciso armazenar [ENTIDADE]
Padrão: [padrão esperado - relacionamento, denormalização]
Problema: [se houver problema de performance/design]

Quero: [migration, otimização, index, etc]
```

### Template 3: Feature
```
[Product Agent]

Requisito: RF-xxx [NOME]
Contexto: [contexto de negócio]
Dúvida: [qual é sua dúvida específica]

Quero: [user stories, roadmap, priorização, etc]
```

## ⚡ 10 Prompts Mais Úteis

1. **Product Agent**
   ```
   "Detalhe a user story para RF-001 com
    acceptance criteria em Gherkin"
   ```

2. **Backend Architect**
   ```
   "Desenhe o diagrama de sequência para
    receber mercadoria e atualizar inventário"
   ```

3. **Database Engineer**
   ```
   "Esta query está lenta. Como otimizar?
    SELECT * FROM inventory WHERE sku_id = ?"
   ```

4. **Frontend & UX**
   ```
   "Implemente página de Picking Order com:
    listagem, formulário, validações"
   ```

5. **Security & Compliance**
   ```
   "Revise este código de JWT.
    Está seguro?"
   ```

6. **DevOps & Infrastructure**
   ```
   "Configure Kubernetes deployment com
    99.95% uptime (3+ replicas, rolling update)"
   ```

7. **Backend Architect**
   ```
   "Qual é a melhor forma de estruturar
    o Allocation Service com CQRS?"
   ```

8. **Database Engineer**
   ```
   "Crie migration EF Core para
    picking_operations com auditoria"
   ```

9. **Product Agent**
   ```
   "Qual é a priorização para Sprint 5?
    Quais features são críticas?"
   ```

10. **Security & Compliance**
    ```
    "Implemente RBAC com 6 roles:
     Admin, Manager, Receiving, Picking, Packing, Shipping"
    ```

## 🔄 Fluxo Diário Recomendado

### Morning (Planejamento)
```
1. Product Agent - Qual é a tarefa do dia?
2. Backend Architect - Qual é a abordagem de design?
3. DevOps - Há algum blocker em CI/CD?
```

### Midday (Desenvolvimento)
```
1. Agente específico - Dúvidas técnicas
2. Security Agent - Validação de segurança
3. Code review com agentes apropriados
```

### Evening (Wrap-up)
```
1. DevOps - Deploy automático em staging
2. Product Agent - Acompanhar progresso vs roadmap
3. Documentar decisões e aprendizados
```

## 📊 Checklist por Sprint

### Sprint 1-2: Foundation
- [ ] **Product Agent** - Detalhe requisitos do Sprint
- [ ] **Backend Architect** - Valide decisões arquitetônicas
- [ ] **Database Engineer** - Crie migrations base
- [ ] **DevOps** - Pipeline CI/CD básico
- [ ] **Security Agent** - Autenticação segura

### Sprint 3-4: Core Features
- [ ] **Product Agent** - User stories com detalhes
- [ ] **Backend Architect** - Agregados bem definidos
- [ ] **Database Engineer** - Schema otimizado
- [ ] **Frontend Agent** - Componentes reutilizáveis
- [ ] **Security Agent** - RBAC implementado

### Sprint 5+: Iterações
- [ ] **Product Agent** - Feedback de usuários
- [ ] Todos agentes - Code review
- [ ] **DevOps** - Performance e monitoring
- [ ] **Security Agent** - Security review

## 🎯 Não Faça Isso

❌ Não implemente sem consultar Product Agent primeiro
❌ Não comite sem validar Security Agent
❌ Não deploys sem passar por DevOps
❌ Não ignore dependências entre features
❌ Não esqueça de testes e documentação

## ✅ Sempre Faça Isso

✅ Comece com Product Agent para entender requisitos
✅ Valide com múltiplos agentes antes de decisões críticas
✅ Use agentes em paralelo para economia de tempo
✅ Documente decisões arquitetônicas (ADRs)
✅ Mantenha agentes atualizados com aprendizados

## 🔗 Links Rápidos

| Recurso | Link |
|---------|------|
| Índice Completo | [INDEX.md](./INDEX.md) |
| ReadMe Geral | [README.md](./README.md) |
| Backend Architect | [backend-architect.md](./backend-architect.md) |
| Database Engineer | [database-engineer.md](./database-engineer.md) |
| Frontend & UX | [frontend-ux.md](./frontend-ux.md) |
| Security & Compliance | [security-compliance.md](./security-compliance.md) |
| DevOps & Infrastructure | [devops-infrastructure.md](./devops-infrastructure.md) |
| Product & Requirements | [product-requirements.md](./product-requirements.md) |

## 🎓 Próximas Ações

1. ✅ Leia este QUICKSTART (2 min)
2. ✅ Leia INDEX.md para visão completa (10 min)
3. ✅ Identifique seu agente primário (1 min)
4. ✅ Formule sua primeira pergunta (5 min)
5. ✅ Comece Sprint 1 com confiança! 🚀

---

**Time:** Tempo para começar
**Complexity:** Fácil de usar (hard to master)
**Impact:** Muito alto na qualidade e velocidade

**👉 Comece AGORA consultando o agente apropriado!**


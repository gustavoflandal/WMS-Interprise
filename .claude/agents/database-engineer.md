# 💾 Database Engineer Agent

## Especialização
Design, otimização, migrações e manutenção de banco de dados. Expertise em PostgreSQL, multi-tenancy, índices, particionamento e performance.

## Responsabilidades Principais

### 1. **Design de Banco de Dados**
- Criar/revisar schema (tabelas, colunas, constraints)
- Normalização e desnormalização estratégica
- Relacionamentos e integridade referencial
- Soft deletes e auditoria

### 2. **Migrations e Versionamento**
- Gerar migrations EF Core (.NET)
- Estratégia de rollback
- Zero-downtime migrations
- Backward compatibility

### 3. **Otimização de Query**
- Análise de EXPLAIN PLAN
- Criação de índices estratégicos
- Rewrite de queries lentas
- Full-text search e Elasticsearch

### 4. **Performance e Escalabilidade**
- Particionamento por tenant/tempo
- Read replicas
- Caching (Redis)
- Connection pooling

### 5. **Multi-tenancy**
- Row Level Security (RLS)
- Isolamento de dados
- Backup por tenant
- Compliance com LGPD

### 6. **Auditoria e Compliance**
- Audit log completo
- Change tracking
- Data retention policies
- GDPR right-to-be-forgotten

## Contexto Documentado

### Documentos Principais (DEVE ESTUDAR)
1. **04_DESIGN_BANCO_DADOS.md**
   - Diagrama ER conceptual
   - 20+ Tabelas do sistema
   - Constraints e validações
   - Índices por performance
   - Particionamento (time-series, tenant)
   - Backup e disaster recovery
   - SQL DDL completo

2. **03_ARQUITETURA_SISTEMA.md**
   - Database per Service pattern
   - Multi-tenancy strategy
   - Performance targets

### Documentos Secundários (REFERÊNCIA)
- 05_ESPECIFICACOES_TECNICAS.md - Padrões de dados
- 10_PERFORMANCE_ESCALABILIDADE.md - Otimizações
- 11_DEPLOYMENT_DEVOPS.md - Backup/restore
- 09_SEGURANCA.md - Encriptação e compliance

## Schema de Banco de Dados

### Dimensões Organizacionais
```sql
-- Tenants (isolamento)
CREATE TABLE tenants (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Warehouses (por tenant)
CREATE TABLE warehouses (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    name VARCHAR(255) NOT NULL,
    location_city VARCHAR(100),
    location_state CHAR(2),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Users (por tenant)
CREATE TABLE users (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(512) NOT NULL,
    email_verified BOOLEAN DEFAULT false,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Roles (RBAC)
CREATE TABLE roles (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    name VARCHAR(100) NOT NULL,
    permissions JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Estrutura de Armazém
```sql
-- Locations (posições de armazenagem)
CREATE TABLE locations (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    aisle VARCHAR(10),
    shelf VARCHAR(10),
    position VARCHAR(10),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(warehouse_id, aisle, shelf, position)
);

-- Storage Types (tipos de armazenagem)
CREATE TABLE storage_types (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    name VARCHAR(100),
    capacity_cubic_meters DECIMAL(10,2),
    max_weight_kg DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Produtos e Inventário
```sql
-- SKUs (Stock Keeping Unit)
CREATE TABLE skus (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    sku_code VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255),
    category_id UUID,
    weight_kg DECIMAL(10,2),
    volume_cubic_meters DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inventory Master (posição atual)
CREATE TABLE inventory_master (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    sku_id UUID NOT NULL REFERENCES skus(id),
    location_id UUID REFERENCES locations(id),
    quantity BIGINT NOT NULL DEFAULT 0,
    reserved_quantity BIGINT NOT NULL DEFAULT 0,
    available_quantity BIGINT GENERATED ALWAYS AS (quantity - reserved_quantity) STORED,
    last_movement_at TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inventory Transactions (histórico)
CREATE TABLE inventory_transactions (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    sku_id UUID NOT NULL REFERENCES skus(id),
    location_id UUID REFERENCES locations(id),
    transaction_type VARCHAR(50),
    quantity_change BIGINT,
    reference_id UUID,
    reference_type VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL REFERENCES users(id)
) PARTITION BY RANGE (created_at);
```

### Operações Inbound (Recebimento)
```sql
-- Advance Shipping Notices (ASN)
CREATE TABLE inbound_asn (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    supplier_id UUID NOT NULL REFERENCES suppliers(id),
    reference_number VARCHAR(100),
    expected_arrival_date TIMESTAMP,
    status VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ASN Lines (itens da nota)
CREATE TABLE inbound_asn_lines (
    id UUID PRIMARY KEY,
    inbound_asn_id UUID NOT NULL REFERENCES inbound_asn(id),
    sku_id UUID NOT NULL REFERENCES skus(id),
    quantity BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Receiving Operations (processo de recebimento)
CREATE TABLE receiving_operations (
    id UUID PRIMARY KEY,
    inbound_asn_id UUID NOT NULL REFERENCES inbound_asn(id),
    receiving_user_id UUID NOT NULL REFERENCES users(id),
    quantity_received BIGINT,
    status VARCHAR(50),
    received_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Operações Outbound (Expedição)
```sql
-- Orders (pedidos)
CREATE TABLE orders (
    id UUID PRIMARY KEY,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),
    customer_id UUID NOT NULL REFERENCES customers(id),
    order_date TIMESTAMP,
    due_date TIMESTAMP,
    status VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Order Lines (itens do pedido)
CREATE TABLE order_lines (
    id UUID PRIMARY KEY,
    order_id UUID NOT NULL REFERENCES orders(id),
    sku_id UUID NOT NULL REFERENCES skus(id),
    quantity BIGINT NOT NULL,
    status VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Picking Orders (ordens de separação)
CREATE TABLE picking_orders (
    id UUID PRIMARY KEY,
    order_id UUID NOT NULL REFERENCES orders(id),
    picking_user_id UUID REFERENCES users(id),
    status VARCHAR(50),
    picked_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Picking Lines (itens a separar)
CREATE TABLE picking_lines (
    id UUID PRIMARY KEY,
    picking_order_id UUID NOT NULL REFERENCES picking_orders(id),
    order_line_id UUID NOT NULL REFERENCES order_lines(id),
    location_id UUID NOT NULL REFERENCES locations(id),
    quantity_picked BIGINT,
    picked_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Packages (pacotes para envio)
CREATE TABLE packages (
    id UUID PRIMARY KEY,
    picking_order_id UUID NOT NULL REFERENCES picking_orders(id),
    package_weight_kg DECIMAL(10,2),
    package_volume_cubic_meters DECIMAL(10,2),
    packed_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Shipments (envios)
CREATE TABLE shipments (
    id UUID PRIMARY KEY,
    package_id UUID NOT NULL REFERENCES packages(id),
    tracking_number VARCHAR(100),
    carrier VARCHAR(100),
    shipped_at TIMESTAMP,
    delivered_at TIMESTAMP,
    status VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Auditoria
```sql
-- Audit Log (todas operações)
CREATE TABLE audit_log (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    user_id UUID NOT NULL REFERENCES users(id),
    entity_type VARCHAR(100),
    entity_id UUID,
    action VARCHAR(50),
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) PARTITION BY RANGE (created_at);
```

## Índices Estratégicos

```sql
-- Performance na busca de inventário
CREATE INDEX idx_inventory_master_warehouse_sku
ON inventory_master(warehouse_id, sku_id);

CREATE INDEX idx_inventory_master_location
ON inventory_master(location_id);

-- Performance em transações (particionadas)
CREATE INDEX idx_inventory_transactions_warehouse_sku
ON inventory_transactions(warehouse_id, sku_id) INCLUDE (quantity_change);

-- Performance em pedidos
CREATE INDEX idx_orders_warehouse_status
ON orders(warehouse_id, status, created_at DESC);

CREATE INDEX idx_order_lines_order_sku
ON order_lines(order_id, sku_id);

-- Performance em picking
CREATE INDEX idx_picking_orders_warehouse_status
ON picking_orders(order_id, status);

-- Performance em buscas de auditoria
CREATE INDEX idx_audit_log_entity
ON audit_log(entity_type, entity_id, created_at DESC);

CREATE INDEX idx_audit_log_user
ON audit_log(user_id, created_at DESC);

-- Full-text search em SKUs
CREATE INDEX idx_skus_description_fulltext
ON skus USING GIN(to_tsvector('portuguese', description));
```

## Particionamento

### Por Tempo (audit_log e inventory_transactions)
```sql
-- Partições mensais para reduzir tamanho de índices
CREATE TABLE audit_log_2025_01 PARTITION OF audit_log
    FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');

-- Query lê apenas partições relevantes
SELECT * FROM audit_log
WHERE created_at >= '2025-01-01' AND created_at < '2025-02-01';
```

### Por Tenant
```sql
-- Se muito volume por tenant, considerar particionamento
-- Normalmente melhor usar Row Level Security (RLS)
CREATE POLICY tenant_isolation ON inventory_master
    USING (warehouse_id IN (SELECT id FROM warehouses WHERE tenant_id = current_user_id()));
```

## Migrations com EF Core

```csharp
public class AddInventoryTransactionsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "inventory_transactions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                sku_id = table.Column<Guid>(type: "uuid", nullable: false),
                transaction_type = table.Column<string>(type: "text", nullable: false),
                quantity_change = table.Column<long>(type: "bigint", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone",
                    nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                created_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_inventory_transactions", x => x.id);
                table.ForeignKey("fk_inventory_transactions_warehouse",
                    x => x.warehouse_id,
                    "warehouses", "id");
                table.ForeignKey("fk_inventory_transactions_sku",
                    x => x.sku_id,
                    "skus", "id");
            }
        );

        migrationBuilder.CreateIndex(
            name: "idx_inventory_transactions_warehouse_sku",
            table: "inventory_transactions",
            columns: new[] { "warehouse_id", "sku_id" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "inventory_transactions");
    }
}
```

## Exemplos de Prompts

```
1. "Crie uma migration para a tabela de picking_operations com auditoria."

2. "Esta query está lenta:
    SELECT * FROM inventory_master WHERE sku_id = ? AND quantity > 0
    Como otimizar?"

3. "Design a estratégia de particionamento para audit_log.
    Qual é a melhor granularidade (diária/mensal)?"

4. "Como implementar Row Level Security para multi-tenancy?
    Qual é o impacto de performance?"

5. "Revise este schema de inventário. Está normalizado corretamente?"

6. "Qual é o plano de backup e disaster recovery?
    RTO < 1 hora, RPO < 15 minutos?"

7. "Como implementar soft deletes mantendo integridade referencial?"

8. "Quais índices são necessários para suportar 50.000 tx/sec?"
```

## Fluxo de Trabalho Típico

### 1. **Análise de Requisito**
- Entender entidades de negócio
- Identificar relacionamentos
- Mapear para tabelas

### 2. **Design de Schema**
- Normalização 3FN+
- Constraints apropriados
- Índices estratégicos

### 3. **Criação de Migration**
- Gerar via EF Core
- Revisar SQL gerado
- Testar rollback

### 4. **Otimização**
- Executar EXPLAIN PLAN
- Adicionar índices se necessário
- Verificar bloat

### 5. **Validação**
- Testar em ambiente de staging
- Validar performance
- Verificar compliance

## Checklist de Qualidade de Schema

- [ ] Todas tabelas têm primary key?
- [ ] Foreign keys têm índices?
- [ ] Constraints apropriados definidos?
- [ ] Soft deletes implementados?
- [ ] Audit log funciona?
- [ ] Multi-tenancy está isolado (RLS)?
- [ ] Índices estão todos criados?
- [ ] Queries têm bom plano?
- [ ] Particionamento está OK?
- [ ] Backup/restore foi testado?

## Integração com Outros Agentes

```
Database Engineer
    ↓
    ├─→ Backend Architect (valida agregados vs schema)
    ├─→ Backend Developers (implementam queries)
    ├─→ DevOps (backup, restore, monitoring)
    └─→ Security & Compliance (auditoria, LGPD)
```

## Responsabilidades Diárias

- Revisar PRs de migrations
- Otimizar queries lentas
- Monitorar índices
- Validar backups
- Documentar schema
- Responder dúvidas de design

## Conhecimento Esperado

- PostgreSQL (versão 14+)
- SQL avançado (CTEs, window functions)
- Entity Framework Core
- Índices e query plans
- Particionamento
- Replicação e HA
- Backup e restore
- Performance tuning
- LGPD/GDPR

---

**Versão:** 1.0
**Criado:** Novembro 2025
**Status:** Ativo
**Próxima Revisão:** Após Sprint 3 (primeira migration)

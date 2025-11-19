# DESIGN DE BANCO DE DADOS - WMS ENTERPRISE

## 1. Princípios de Design

### 1.1 Multi-tenancy

- Isolamento total de dados por tenant
- Row Level Security (RLS) no PostgreSQL
- Chave `tenant_id` em todas as tabelas principais
- Particionamento lógico e físico por tenant

### 1.2 Auditoria Completa

- Todas as alterações registradas em `audit_log`
- Campos: `created_at`, `updated_at`, `created_by`, `updated_by`
- Histórico de mudanças sem perda de dados

### 1.3 Soft Deletes

- Registros marcados como deletados, não removidos
- Coluna `deleted_at` para controle
- Queries filtram automaticamente deletados

---

## 2. Diagrama ER (Conceptual)

```
┌──────────────┐
│   TENANTS    │ (Organizações/Depositantes)
└──────┬───────┘
       │ 1
       │
       │ N
┌──────▼──────────────────────────────────────────────┐
│                   WAREHOUSES                        │
│ (Armazéns/Centros de Distribuição)                 │
└──────┬──────────────────────────────────────────────┘
       │
       ├─── STOCKS (Estoques - podem ser por cliente ou compartilhados)
       │     └─── LOCATIONS (Aisles, Racks, Shelves dentro do estoque)
       │
       ├─── USER_WAREHOUSES (Usuários vinculados a armazéns)
       │     └─── USER_WAREHOUSE_ROLES (Roles e permissões por armazém)
       │
       └─── STORAGE_TYPES (Tipo de estrutura)

Nota: Sistema multi-armazém completo
- Usuários podem ter acesso a múltiplos armazéns
- Roles e permissões são atribuídas por armazém
- Cada armazém contém múltiplos estoques
- Estoques podem ser dedicados (um cliente) ou compartilhados
- Movimentações podem ocorrer entre estoques e armazéns
```

---

## 3. Tabelas do Sistema

### 3.1 Dimensões Organizacionais

#### TABLE: tenants
```sql
CREATE TABLE tenants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(255) NOT NULL,
    razao_social VARCHAR(500),
    cnpj VARCHAR(18) UNIQUE,
    email VARCHAR(255),
    telefone VARCHAR(20),

    -- Configurações
    max_armazens INT DEFAULT 5,
    max_usuarios INT DEFAULT 100,
    max_localizacoes_estoque INT DEFAULT 100000,
    habilitar_multi_armazem BOOLEAN DEFAULT FALSE,
    habilitar_acesso_api BOOLEAN DEFAULT TRUE,

    -- Status
    status ENUM ('ATIVO', 'SUSPENSO', 'DELETADO') DEFAULT 'ATIVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deletado_em TIMESTAMP,

    CONSTRAINT check_cnpj_format CHECK (cnpj ~ '^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$' OR cnpj IS NULL)
);
```

#### TABLE: warehouses
```sql
CREATE TABLE warehouses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    -- Informações Básicas
    nome VARCHAR(255) NOT NULL,
    codigo VARCHAR(50) NOT NULL,
    descricao TEXT,

    -- Localização
    endereco VARCHAR(500),
    cidade VARCHAR(100),
    estado VARCHAR(2),
    cep VARCHAR(10),
    pais VARCHAR(3) DEFAULT 'BRA',
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),

    -- Capacidade
    total_posicoes INT,
    capacidade_peso_total DECIMAL(15,2),  -- em kg

    -- Operação
    horario_abertura TIME,
    horario_fechamento TIME,
    max_trabalhadores INT,

    -- Status
    status ENUM ('ATIVO', 'INATIVO', 'EM_MANUTENCAO') DEFAULT 'ATIVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    criado_por UUID,
    atualizado_por UUID,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    UNIQUE(tenant_id, codigo)
);
```

#### TABLE: user_warehouses
```sql
-- Relacionamento N:N entre usuários e armazéns
CREATE TABLE user_warehouses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID NOT NULL,
    armazem_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    -- Status do vínculo
    status ENUM ('ATIVO', 'INATIVO') DEFAULT 'ATIVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    criado_por UUID,

    FOREIGN KEY (usuario_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id) ON DELETE CASCADE,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),

    UNIQUE(usuario_id, armazem_id)
);

CREATE INDEX idx_user_warehouses_user ON user_warehouses(usuario_id);
CREATE INDEX idx_user_warehouses_warehouse ON user_warehouses(armazem_id);
```

#### TABLE: user_warehouse_roles
```sql
-- Atribuição de roles e permissões específicas por armazém
CREATE TABLE user_warehouse_roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_armazem_id UUID NOT NULL,
    role_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    -- Permissões específicas para este armazém (sobrepõe permissoes_base da role)
    permissoes_especificas_armazem JSONB,

    -- Pode restringir ainda mais as permissões da role base
    restricoes_permissoes JSONB,

    -- Status
    status ENUM ('ATIVO', 'INATIVO') DEFAULT 'ATIVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    criado_por UUID,

    FOREIGN KEY (usuario_armazem_id) REFERENCES user_warehouses(id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES roles(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),

    UNIQUE(usuario_armazem_id, role_id)
);

CREATE INDEX idx_user_warehouse_roles_uw ON user_warehouse_roles(usuario_armazem_id);
CREATE INDEX idx_user_warehouse_roles_role ON user_warehouse_roles(role_id);

-- Exemplo de estrutura de permissões em JSONB:
-- {
--   "modulos": {
--     "recebimento": {"criar": true, "ler": true, "atualizar": true, "deletar": false},
--     "expedicao": {"criar": true, "ler": true, "atualizar": true, "deletar": false},
--     "inventario": {"criar": false, "ler": true, "atualizar": false, "deletar": false},
--     "relatorios": {"criar": false, "ler": true, "atualizar": false, "deletar": false}
--   },
--   "funcionalidades": {
--     "pode_sobrescrever_localizacao": false,
--     "pode_cancelar_pedidos": true,
--     "pode_ajustar_inventario": false
--   }
-- }
```

---

### 3.2 Estrutura de Armazém

#### TABLE: stocks
```sql
-- Estoques dentro de armazéns
-- Cada armazém pode ter múltiplos estoques
-- Estoques podem ser dedicados a um cliente ou compartilhados
CREATE TABLE stocks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    armazem_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    -- Identificação
    codigo VARCHAR(50) NOT NULL,
    nome VARCHAR(255) NOT NULL,
    descricao TEXT,

    -- Tipo de Estoque
    tipo_estoque ENUM ('DEDICADO', 'COMPARTILHADO', 'QUARENTENA', 'DEVOLUCOES') DEFAULT 'COMPARTILHADO',

    -- Cliente (para estoques dedicados)
    cliente_id UUID,  -- NULL se for compartilhado

    -- Estrutura Física
    tipo_armazenagem_id UUID,  -- Tipo de estrutura predominante
    capacidade_total_posicoes INT,
    capacidade_total_peso DECIMAL(15,2),  -- kg
    capacidade_total_volume DECIMAL(15,2),  -- m³

    -- Características Operacionais
    permite_produtos_mistos BOOLEAN DEFAULT TRUE,
    permite_lotes_mistos BOOLEAN DEFAULT TRUE,
    requer_verificacao_qualidade BOOLEAN DEFAULT FALSE,
    exigir_fifo BOOLEAN DEFAULT TRUE,
    exigir_fefo BOOLEAN DEFAULT FALSE,

    -- Controle de Temperatura e Ambiente
    temperatura_controlada BOOLEAN DEFAULT FALSE,
    temperatura_min DECIMAL(5,2),
    temperatura_max DECIMAL(5,2),
    umidade_controlada BOOLEAN DEFAULT FALSE,
    umidade_min DECIMAL(5,2),
    umidade_max DECIMAL(5,2),

    -- Categorias Permitidas/Bloqueadas
    categorias_produto_permitidas UUID[],
    categorias_produto_bloqueadas UUID[],
    permite_materiais_perigosos BOOLEAN DEFAULT FALSE,

    -- Status e Capacidade Atual
    status ENUM ('ATIVO', 'INATIVO', 'CHEIO', 'MANUTENCAO', 'FECHADO') DEFAULT 'ATIVO',
    posicoes_ocupadas_atual INT DEFAULT 0,
    peso_atual DECIMAL(15,2) DEFAULT 0,
    volume_atual DECIMAL(15,2) DEFAULT 0,
    percentual_utilizacao DECIMAL(5,2) GENERATED ALWAYS AS (
        CASE
            WHEN capacidade_total_posicoes > 0
            THEN (posicoes_ocupadas_atual::DECIMAL / capacidade_total_posicoes) * 100
            ELSE 0
        END
    ) STORED,

    -- Auditoria
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    criado_por UUID,
    atualizado_por UUID,

    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (cliente_id) REFERENCES customers(id),
    FOREIGN KEY (tipo_armazenagem_id) REFERENCES storage_types(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),
    FOREIGN KEY (atualizado_por) REFERENCES users(id),

    UNIQUE(armazem_id, codigo)
);

CREATE INDEX idx_stocks_warehouse ON stocks(armazem_id);
CREATE INDEX idx_stocks_customer ON stocks(cliente_id);
CREATE INDEX idx_stocks_type ON stocks(tipo_estoque);
CREATE INDEX idx_stocks_status ON stocks(status);
```

#### TABLE: locations
```sql
CREATE TABLE locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    armazem_id UUID NOT NULL,
    estoque_id UUID NOT NULL,  -- Locations agora pertencem a um estoque
    tenant_id UUID NOT NULL,

    -- Identificação
    codigo_corredor VARCHAR(10),         -- Corredor (A, B, C...)
    codigo_rack VARCHAR(10),             -- Prateleira (1, 2, 3...)
    codigo_nivel VARCHAR(10),            -- Nível (1, 2, 3...)
    codigo_compartimento VARCHAR(10),    -- Compartimento (A, B, C...)

    codigo_localizacao VARCHAR(50) GENERATED ALWAYS AS (
        CONCAT(codigo_corredor, '-', codigo_rack, '-', codigo_nivel, '-', codigo_compartimento)
    ) STORED,

    -- Características Físicas
    tipo_armazenagem_id UUID,
    peso_maximo DECIMAL(12,2),       -- kg
    volume_maximo DECIMAL(12,2),     -- m³
    altura_cm INT,
    largura_cm INT,
    profundidade_cm INT,

    -- Estado
    status ENUM ('DISPONIVEL', 'CHEIO', 'RESERVADO', 'BLOQUEADO', 'MANUTENCAO') DEFAULT 'DISPONIVEL',
    disponivel_para_armazenagem BOOLEAN DEFAULT TRUE,
    peso_atual DECIMAL(12,2) DEFAULT 0,
    volume_atual DECIMAL(12,2) DEFAULT 0,

    -- Regras
    categorias_produto_permitidas UUID[],
    categorias_produto_bloqueadas UUID[],

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (estoque_id) REFERENCES stocks(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (tipo_armazenagem_id) REFERENCES storage_types(id),

    UNIQUE(armazem_id, estoque_id, codigo_corredor, codigo_rack, codigo_nivel, codigo_compartimento)
);

CREATE INDEX idx_locations_warehouse ON locations(armazem_id);
CREATE INDEX idx_locations_stock ON locations(estoque_id);
CREATE INDEX idx_locations_status ON locations(status);
CREATE INDEX idx_locations_code ON locations(codigo_localizacao);
```

#### TABLE: storage_types
```sql
CREATE TABLE storage_types (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    nome VARCHAR(100),              -- "Rack Convencional", "Cantilever", "Drive-in"
    codigo VARCHAR(20) UNIQUE,

    -- Características
    pode_armazenar_paletes BOOLEAN DEFAULT TRUE,
    pode_armazenar_caixas BOOLEAN DEFAULT TRUE,
    pode_armazenar_itens_pequenos BOOLEAN DEFAULT FALSE,

    -- Climático
    temperatura_min DECIMAL(5,2),
    temperatura_max DECIMAL(5,2),
    controle_umidade BOOLEAN DEFAULT FALSE,

    -- Operacional
    prioridade_separacao INT,           -- Prioridade de uso (1=máxima)
    taxa_utilizacao DECIMAL(5,2),       -- Taxa de utilização esperada (%)

    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id)
);
```

---

### 3.3 Produtos e Inventário

#### TABLE: skus (Stock Keeping Units)
```sql
CREATE TABLE skus (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    -- Identificação
    codigo_sku VARCHAR(50) NOT NULL,
    ean13 VARCHAR(13),
    sku_fornecedor VARCHAR(50),

    -- Descrição
    nome VARCHAR(255) NOT NULL,
    descricao TEXT,
    categoria_id UUID,

    -- Características Físicas
    peso_kg DECIMAL(12,3),
    volume_m3 DECIMAL(12,6),
    altura_cm INT,
    largura_cm INT,
    profundidade_cm INT,

    -- Classificação ABC
    categoria_abc ENUM ('A', 'B', 'C') DEFAULT 'C',

    -- Configuração
    tipo_armazenagem_id UUID,           -- Tipo preferido de armazenagem
    requer_controle_temperatura BOOLEAN DEFAULT FALSE,
    temperatura_min DECIMAL(5,2),
    temperatura_max DECIMAL(5,2),
    material_perigoso BOOLEAN DEFAULT FALSE,
    fragil BOOLEAN DEFAULT FALSE,

    -- Validade
    possui_data_validade BOOLEAN DEFAULT FALSE,
    vida_util_dias INT,

    -- Status
    status ENUM ('ATIVO', 'DESCONTINUADO', 'EM_REVISAO') DEFAULT 'ATIVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (categoria_id) REFERENCES product_categories(id),
    FOREIGN KEY (tipo_armazenagem_id) REFERENCES storage_types(id),

    UNIQUE(tenant_id, codigo_sku)
);

CREATE INDEX idx_skus_ean ON skus(ean13);
CREATE INDEX idx_skus_category ON skus(categoria_id);
```

#### TABLE: product_categories
```sql
CREATE TABLE product_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    nome VARCHAR(100) NOT NULL,
    codigo VARCHAR(50),
    descricao TEXT,
    categoria_pai_id UUID,

    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (categoria_pai_id) REFERENCES product_categories(id)
);
```

#### TABLE: inventory_master
```sql
CREATE TABLE inventory_master (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    armazem_id UUID NOT NULL,
    estoque_id UUID NOT NULL,  -- Inventário agora pertence a um estoque

    sku_id UUID NOT NULL,
    localizacao_id UUID NOT NULL,
    lote_id UUID,

    -- Quantidades
    quantidade_em_maos INT DEFAULT 0,
    quantidade_reservada INT DEFAULT 0,
    quantidade_disponivel INT GENERATED ALWAYS AS (quantidade_em_maos - quantidade_reservada) STORED,

    -- Rastreabilidade
    numero_lote VARCHAR(100),
    numeros_serie TEXT[],          -- Array de números de série
    data_validade DATE,
    data_fabricacao DATE,

    -- Status
    status ENUM ('ATIVO', 'BLOQUEADO', 'VENCIDO', 'DANIFICADO', 'OBSOLETO') DEFAULT 'ATIVO',

    -- Controle
    ultima_movimentacao_em TIMESTAMP,
    ultima_contagem_em TIMESTAMP,

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (estoque_id) REFERENCES stocks(id),
    FOREIGN KEY (sku_id) REFERENCES skus(id),
    FOREIGN KEY (localizacao_id) REFERENCES locations(id),

    UNIQUE(armazem_id, estoque_id, sku_id, localizacao_id, lote_id)
);

CREATE INDEX idx_inventory_sku ON inventory_master(sku_id);
CREATE INDEX idx_inventory_warehouse ON inventory_master(armazem_id);
CREATE INDEX idx_inventory_stock ON inventory_master(estoque_id);
CREATE INDEX idx_inventory_location ON inventory_master(localizacao_id);
CREATE INDEX idx_inventory_expiration ON inventory_master(data_validade);
CREATE INDEX idx_inventory_status ON inventory_master(status);
```

#### TABLE: inventory_transactions
```sql
CREATE TABLE inventory_transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    -- Referência
    inventario_master_id UUID NOT NULL,
    documento_id UUID,               -- Referência para documento (ASN, Pedido Separação, etc)

    -- Tipo de Movimento
    tipo_transacao ENUM (
        'RECEBIMENTO_ENTRADA',
        'SEPARACAO_SAIDA',
        'TRANSFERENCIA',              -- Transferência entre localizações
        'TRANSFERENCIA_ESTOQUE',      -- Transferência entre estoques
        'TRANSFERENCIA_ARMAZEM',      -- Transferência entre armazéns
        'AJUSTE',
        'CONTAGEM',
        'DESCARTE',
        'DEVOLUCAO'
    ),

    -- Valores
    quantidade_antes INT,
    quantidade_depois INT,
    quantidade_movida INT GENERATED ALWAYS AS (quantidade_depois - quantidade_antes) STORED,

    -- Origem e Destino (para movimentações)
    armazem_origem_id UUID,
    armazem_destino_id UUID,
    estoque_origem_id UUID,
    estoque_destino_id UUID,
    localizacao_origem_id UUID,
    localizacao_destino_id UUID,

    -- Detalhes
    motivo TEXT,
    observacoes TEXT,
    numero_referencia VARCHAR(100),

    -- Auditoria
    criado_por UUID,
    role_usuario VARCHAR(50),
    aprovado_por UUID,
    aprovado_em TIMESTAMP,

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (inventario_master_id) REFERENCES inventory_master(id),
    FOREIGN KEY (armazem_origem_id) REFERENCES warehouses(id),
    FOREIGN KEY (armazem_destino_id) REFERENCES warehouses(id),
    FOREIGN KEY (estoque_origem_id) REFERENCES stocks(id),
    FOREIGN KEY (estoque_destino_id) REFERENCES stocks(id),
    FOREIGN KEY (localizacao_origem_id) REFERENCES locations(id),
    FOREIGN KEY (localizacao_destino_id) REFERENCES locations(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),
    FOREIGN KEY (aprovado_por) REFERENCES users(id)
);

CREATE INDEX idx_inventory_transactions_inventory ON inventory_transactions(inventario_master_id);
CREATE INDEX idx_inventory_transactions_date ON inventory_transactions(criado_em DESC);
CREATE INDEX idx_inventory_transactions_user ON inventory_transactions(criado_por);
CREATE INDEX idx_inventory_transactions_type ON inventory_transactions(tipo_transacao);
CREATE INDEX idx_inventory_transactions_warehouse_from ON inventory_transactions(armazem_origem_id);
CREATE INDEX idx_inventory_transactions_warehouse_to ON inventory_transactions(armazem_destino_id);
CREATE INDEX idx_inventory_transactions_stock_from ON inventory_transactions(estoque_origem_id);
CREATE INDEX idx_inventory_transactions_stock_to ON inventory_transactions(estoque_destino_id);
```

---

### 3.4 Operações Inbound (Recebimento)

#### TABLE: inbound_asn (Aviso de Recebimento)
```sql
CREATE TABLE inbound_asn (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    armazem_id UUID NOT NULL,

    -- Identificação
    numero_asn VARCHAR(50) NOT NULL,
    numero_po VARCHAR(50),
    fornecedor_id UUID NOT NULL,

    -- Datas
    data_chegada_prevista DATE,
    data_chegada_real DATE,

    -- Transportação
    nome_transportadora VARCHAR(255),
    referencia_transporte VARCHAR(100),
    placa_veiculo VARCHAR(20),

    -- Quantidades
    linhas_esperadas INT,
    paletes_esperados INT,

    -- Status
    status ENUM (
        'RASCUNHO',
        'AGENDADO',
        'CHEGOU',
        'RECEBIMENTO_EM_ANDAMENTO',
        'VERIFICACAO_QUALIDADE',
        'TOTALMENTE_RECEBIDO',
        'PARCIALMENTE_RECEBIDO',
        'FECHADO',
        'CANCELADO'
    ) DEFAULT 'RASCUNHO',

    -- Controle
    linhas_recebidas INT DEFAULT 0,
    linhas_rejeitadas INT DEFAULT 0,
    quantidade_recebida INT DEFAULT 0,

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    recebido_em TIMESTAMP,
    fechado_em TIMESTAMP,
    criado_por UUID,
    recebido_por UUID,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (fornecedor_id) REFERENCES suppliers(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),
    FOREIGN KEY (recebido_por) REFERENCES users(id),

    UNIQUE(tenant_id, numero_asn)
);
```

#### TABLE: inbound_asn_lines
```sql
CREATE TABLE inbound_asn_lines (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    asn_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    numero_linha INT,
    sku_id UUID NOT NULL,

    -- Quantidades
    quantidade_esperada INT,
    unidade_esperada VARCHAR(10),
    quantidade_recebida INT DEFAULT 0,
    quantidade_rejeitada INT DEFAULT 0,

    -- Lote/Série
    numero_lote VARCHAR(100),
    numeros_serie TEXT[],
    data_validade DATE,

    -- Status
    status ENUM ('PENDENTE', 'RECEBIDO', 'PARCIALMENTE_RECEBIDO', 'REJEITADO', 'CANCELADO') DEFAULT 'PENDENTE',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    recebido_em TIMESTAMP,

    FOREIGN KEY (asn_id) REFERENCES inbound_asn(id) ON DELETE CASCADE,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (sku_id) REFERENCES skus(id)
);
```

#### TABLE: receiving_operations
```sql
CREATE TABLE receiving_operations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    asn_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    -- Operação
    numero_operacao VARCHAR(50),
    status ENUM ('EM_ANDAMENTO', 'CONCLUIDO', 'FALHOU', 'CANCELADO') DEFAULT 'EM_ANDAMENTO',

    -- Qualidade
    verificacao_qualidade_obrigatoria BOOLEAN DEFAULT FALSE,
    qualidade_aprovada BOOLEAN,
    observacoes_qualidade TEXT,
    qualidade_verificada_por UUID,
    qualidade_verificada_em TIMESTAMP,

    -- Localização
    doca_recebimento_id UUID,
    localizacao_preparacao_id UUID,
    localizacao_final_id UUID,

    -- Timestamps
    iniciado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    concluido_em TIMESTAMP,
    iniciado_por UUID,
    concluido_por UUID,

    FOREIGN KEY (asn_id) REFERENCES inbound_asn(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (iniciado_por) REFERENCES users(id),
    FOREIGN KEY (concluido_por) REFERENCES users(id)
);
```

---

### 3.5 Operações Outbound (Picking, Packing, Shipping)

#### TABLE: orders
```sql
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    armazem_id UUID NOT NULL,

    -- Identificação
    numero_pedido VARCHAR(50) NOT NULL,
    data_pedido DATE NOT NULL,
    cliente_id UUID NOT NULL,

    -- Datas
    data_entrega_prometida DATE,
    data_limite_expedicao DATE,

    -- Destinatário
    endereco_entrega VARCHAR(500),
    cidade_entrega VARCHAR(100),
    estado_entrega VARCHAR(2),
    cep_entrega VARCHAR(10),

    -- Faturamento
    total_linhas INT,
    total_unidades INT,
    peso_total DECIMAL(12,2),
    valor_total DECIMAL(15,2),

    -- Prioridade e Flags
    prioridade INT DEFAULT 0,          -- 0=normal, 1=urgente, etc
    eh_presente BOOLEAN DEFAULT FALSE,
    eh_fragil BOOLEAN DEFAULT FALSE,
    requer_assinatura BOOLEAN DEFAULT FALSE,

    -- Status
    status_pedido ENUM (
        'NOVO',
        'ALOCADO',
        'PRONTO_SEPARAR',
        'SEPARACAO_EM_ANDAMENTO',
        'SEPARADO',
        'EMBALAGEM_EM_ANDAMENTO',
        'EMBALADO',
        'PRONTO_EXPEDIR',
        'EXPEDIDO',
        'ENTREGUE',
        'CANCELADO',
        'EM_ESPERA'
    ) DEFAULT 'NOVO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    cancelado_em TIMESTAMP,
    expedido_em TIMESTAMP,
    entregue_em TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (cliente_id) REFERENCES customers(id),

    UNIQUE(tenant_id, numero_pedido)
);

CREATE INDEX idx_orders_status ON orders(status_pedido);
CREATE INDEX idx_orders_date ON orders(data_pedido);
CREATE INDEX idx_orders_delivery_date ON orders(data_entrega_prometida);
```

#### TABLE: order_lines
```sql
CREATE TABLE order_lines (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    numero_linha INT,
    sku_id UUID NOT NULL,

    -- Quantidades
    quantidade_pedida INT,
    quantidade_alocada INT DEFAULT 0,
    quantidade_separada INT DEFAULT 0,
    quantidade_embalada INT DEFAULT 0,
    quantidade_expedida INT DEFAULT 0,

    -- Preço Unitário
    preco_unitario DECIMAL(12,2),
    total_linha DECIMAL(15,2),

    -- Status
    status_linha ENUM ('ABERTO', 'ALOCADO', 'PRONTO_SEPARAR', 'SEPARADO', 'EMBALADO', 'EXPEDIDO', 'CANCELADO') DEFAULT 'ABERTO',

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (pedido_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (sku_id) REFERENCES skus(id)
);
```

#### TABLE: picking_orders
```sql
CREATE TABLE picking_orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id UUID NOT NULL,
    tenant_id UUID NOT NULL,
    armazem_id UUID NOT NULL,

    -- Identificação
    id_separacao VARCHAR(50) NOT NULL,
    onda_id UUID,

    -- Atribuição
    atribuido_usuario_id UUID,
    atribuido_em TIMESTAMP,

    -- Execução
    iniciado_em TIMESTAMP,
    concluido_em TIMESTAMP,

    -- Quantidades
    total_linhas INT,
    linhas_concluidas INT DEFAULT 0,

    -- Rota
    coordenadas_rota_sugerida JSONB,  -- JSON com sequência otimizada de localizações
    distancia_metros INT,
    tempo_estimado_minutos INT,

    -- Status
    status ENUM ('CRIADO', 'ATRIBUIDO', 'EM_ANDAMENTO', 'CONCLUIDO', 'CANCELADO') DEFAULT 'CRIADO',

    -- Performance
    distancia_real_metros INT,
    tempo_real_minutos INT,
    contagem_erros INT DEFAULT 0,

    FOREIGN KEY (pedido_id) REFERENCES orders(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),
    FOREIGN KEY (atribuido_usuario_id) REFERENCES users(id),

    UNIQUE(tenant_id, id_separacao)
);
```

#### TABLE: picking_lines
```sql
CREATE TABLE picking_lines (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_separacao_id UUID NOT NULL,
    linha_pedido_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    sku_id UUID NOT NULL,
    localizacao_id UUID NOT NULL,

    -- Quantidades
    quantidade_requerida INT,
    quantidade_separada INT DEFAULT 0,

    -- Sequência
    numero_sequencia INT,

    -- Status
    status ENUM ('PENDENTE', 'SEPARADO', 'VERIFICADO', 'CANCELADO') DEFAULT 'PENDENTE',

    -- Auditoria
    separado_em TIMESTAMP,
    verificado_em TIMESTAMP,
    separado_por UUID,
    verificado_por UUID,

    FOREIGN KEY (pedido_separacao_id) REFERENCES picking_orders(id) ON DELETE CASCADE,
    FOREIGN KEY (linha_pedido_id) REFERENCES order_lines(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (sku_id) REFERENCES skus(id),
    FOREIGN KEY (localizacao_id) REFERENCES locations(id),
    FOREIGN KEY (separado_por) REFERENCES users(id),
    FOREIGN KEY (verificado_por) REFERENCES users(id)
);
```

#### TABLE: packages
```sql
CREATE TABLE packages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    -- Identificação
    numero_pacote VARCHAR(50) NOT NULL,
    numero_rastreamento VARCHAR(100),

    -- Dimensões e Peso
    peso_kg DECIMAL(12,3),
    largura_cm INT,
    comprimento_cm INT,
    altura_cm INT,
    volume_m3 DECIMAL(12,6),

    -- Embalagem
    tipo_embalagem VARCHAR(50),       -- 'Caixa', 'Envelope', 'Palete', etc

    -- Status
    status ENUM ('PREPARADO', 'ETIQUETADO', 'CONSOLIDADO', 'EXPEDIDO', 'ENTREGUE', 'DEVOLVIDO') DEFAULT 'PREPARADO',

    -- Transportação
    transportadora_id UUID,
    metodo_envio_id UUID,
    custo_envio DECIMAL(12,2),

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    embalado_em TIMESTAMP,
    expedido_em TIMESTAMP,
    entregue_em TIMESTAMP,

    FOREIGN KEY (pedido_id) REFERENCES orders(id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),

    UNIQUE(tenant_id, numero_pacote)
);
```

#### TABLE: shipments
```sql
CREATE TABLE shipments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    armazem_id UUID NOT NULL,

    -- Identificação
    numero_expedicao VARCHAR(50) NOT NULL,
    numero_manifesto VARCHAR(50),

    -- Consolidação
    total_pacotes INT,
    peso_total DECIMAL(12,2),
    volume_total DECIMAL(12,6),
    total_pedidos INT,

    -- Transportação
    transportadora_id UUID,
    metodo_envio_id UUID,
    veiculo_id VARCHAR(50),

    -- Datas
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    preparado_em TIMESTAMP,
    manifestado_em TIMESTAMP,
    despachado_em TIMESTAMP,

    -- Status
    status ENUM ('PREPARACAO', 'PRONTO', 'MANIFESTADO', 'DESPACHADO', 'EM_TRANSITO', 'ENTREGUE', 'CANCELADO') DEFAULT 'PREPARACAO',

    -- Rastreamento
    referencia_tms VARCHAR(100),
    url_rastreamento TEXT,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_id) REFERENCES warehouses(id),

    UNIQUE(tenant_id, numero_expedicao)
);
```

---

### 3.6 Transferências entre Estoques e Armazéns

#### TABLE: stock_transfers
```sql
-- Controla transferências entre estoques (mesmo armazém ou armazéns diferentes)
CREATE TABLE stock_transfers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    -- Identificação
    numero_transferencia VARCHAR(50) NOT NULL,
    tipo_transferencia ENUM ('INTERNA', 'INTER_ARMAZEM', 'INTER_ESTOQUE', 'CONSOLIDACAO') DEFAULT 'INTERNA',

    -- Origem
    armazem_origem_id UUID NOT NULL,
    estoque_origem_id UUID NOT NULL,

    -- Destino
    armazem_destino_id UUID NOT NULL,
    estoque_destino_id UUID NOT NULL,

    -- Status
    status ENUM (
        'RASCUNHO',
        'APROVACAO_PENDENTE',
        'APROVADO',
        'EM_TRANSITO',
        'PARCIALMENTE_RECEBIDO',
        'CONCLUIDO',
        'CANCELADO'
    ) DEFAULT 'RASCUNHO',

    -- Quantidades
    total_linhas INT,
    quantidade_total INT,
    quantidade_recebida INT DEFAULT 0,

    -- Razão
    motivo TEXT,
    observacoes TEXT,

    -- Datas
    data_agendada DATE,
    aprovado_em TIMESTAMP,
    iniciado_em TIMESTAMP,
    concluido_em TIMESTAMP,

    -- Auditoria
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    criado_por UUID,
    aprovado_por UUID,
    executado_por UUID,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (armazem_origem_id) REFERENCES warehouses(id),
    FOREIGN KEY (armazem_destino_id) REFERENCES warehouses(id),
    FOREIGN KEY (estoque_origem_id) REFERENCES stocks(id),
    FOREIGN KEY (estoque_destino_id) REFERENCES stocks(id),
    FOREIGN KEY (criado_por) REFERENCES users(id),
    FOREIGN KEY (aprovado_por) REFERENCES users(id),
    FOREIGN KEY (executado_por) REFERENCES users(id),

    UNIQUE(tenant_id, numero_transferencia)
);

CREATE INDEX idx_stock_transfers_status ON stock_transfers(status);
CREATE INDEX idx_stock_transfers_warehouse_from ON stock_transfers(armazem_origem_id);
CREATE INDEX idx_stock_transfers_warehouse_to ON stock_transfers(armazem_destino_id);
CREATE INDEX idx_stock_transfers_date ON stock_transfers(data_agendada);
```

#### TABLE: stock_transfer_lines
```sql
CREATE TABLE stock_transfer_lines (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    transferencia_id UUID NOT NULL,
    tenant_id UUID NOT NULL,

    numero_linha INT,
    sku_id UUID NOT NULL,

    -- Origem
    localizacao_origem_id UUID NOT NULL,
    numero_lote VARCHAR(100),
    numeros_serie TEXT[],

    -- Destino
    localizacao_destino_id UUID,  -- Pode ser NULL se ainda não alocado

    -- Quantidades
    quantidade_a_transferir INT,
    quantidade_transferida INT DEFAULT 0,
    quantidade_recebida INT DEFAULT 0,

    -- Status
    status ENUM ('PENDENTE', 'SEPARADO', 'EM_TRANSITO', 'RECEBIDO', 'CANCELADO') DEFAULT 'PENDENTE',

    -- Auditoria
    separado_em TIMESTAMP,
    separado_por UUID,
    recebido_em TIMESTAMP,
    recebido_por UUID,

    -- Timestamps
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (transferencia_id) REFERENCES stock_transfers(id) ON DELETE CASCADE,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (sku_id) REFERENCES skus(id),
    FOREIGN KEY (localizacao_origem_id) REFERENCES locations(id),
    FOREIGN KEY (localizacao_destino_id) REFERENCES locations(id),
    FOREIGN KEY (separado_por) REFERENCES users(id),
    FOREIGN KEY (recebido_por) REFERENCES users(id)
);

CREATE INDEX idx_stock_transfer_lines_transfer ON stock_transfer_lines(transferencia_id);
CREATE INDEX idx_stock_transfer_lines_sku ON stock_transfer_lines(sku_id);
CREATE INDEX idx_stock_transfer_lines_status ON stock_transfer_lines(status);
```

---

### 3.7 Referências Mestras

#### TABLE: suppliers
```sql
CREATE TABLE suppliers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    nome VARCHAR(255) NOT NULL,
    razao_social VARCHAR(500),
    cnpj VARCHAR(18),
    email VARCHAR(255),
    telefone VARCHAR(20),

    endereco VARCHAR(500),
    cidade VARCHAR(100),
    estado VARCHAR(2),

    status ENUM ('ATIVO', 'INATIVO', 'LISTA_NEGRA') DEFAULT 'ATIVO',

    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    UNIQUE(tenant_id, cnpj)
);
```

#### TABLE: customers
```sql
CREATE TABLE customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    nome VARCHAR(255) NOT NULL,
    tipo ENUM ('PJ', 'PF') DEFAULT 'PJ',
    numero_documento VARCHAR(18),

    email VARCHAR(255),
    telefone VARCHAR(20),

    status ENUM ('ATIVO', 'INATIVO', 'BLOQUEADO') DEFAULT 'ATIVO',

    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    UNIQUE(tenant_id, numero_documento)
);
```

---

### 3.8 Auditoria e Compliance

#### TABLE: audit_log
```sql
CREATE TABLE audit_log (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,

    -- Entidade Afetada
    tipo_entidade VARCHAR(100),       -- 'Pedido', 'PedidoSeparacao', 'Inventario', etc
    entidade_id UUID,

    -- Ação
    acao ENUM ('CRIAR', 'ATUALIZAR', 'DELETAR', 'VISUALIZAR', 'EXPORTAR') NOT NULL,
    descricao_acao TEXT,

    -- Mudanças
    valores_antigos JSONB,
    valores_novos JSONB,

    -- Usuário
    usuario_id UUID,
    role_usuario VARCHAR(100),

    -- Localização
    endereco_ip INET,
    user_agent TEXT,

    -- Timestamp
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (tenant_id) REFERENCES tenants(id)
);

CREATE INDEX idx_audit_log_tenant ON audit_log(tenant_id);
CREATE INDEX idx_audit_log_entity ON audit_log(tipo_entidade, entidade_id);
CREATE INDEX idx_audit_log_date ON audit_log(criado_em DESC);
CREATE INDEX idx_audit_log_user ON audit_log(usuario_id);
```

---

## 4. Constraints e Validações

### 4.1 Integridade de Dados

```sql
-- Verificar que quantidade_reservada <= quantidade_em_maos
ALTER TABLE inventory_master ADD CONSTRAINT
    check_reserva VALIDATE AS
    quantidade_reservada <= quantidade_em_maos;

-- Verificar que datas de entrega são futuras
ALTER TABLE orders ADD CONSTRAINT
    check_data_entrega AS
    data_entrega_prometida >= data_pedido;

-- Verificar que peso e volume são positivos
ALTER TABLE skus ADD CONSTRAINT
    check_peso_volume AS
    peso_kg > 0 AND volume_m3 > 0;
```

---

## 5. Índices por Performance

```sql
-- Recebimento
CREATE INDEX idx_inbound_asn_status ON inbound_asn(status);
CREATE INDEX idx_inbound_asn_warehouse ON inbound_asn(armazem_id);
CREATE INDEX idx_inbound_asn_supplier ON inbound_asn(fornecedor_id);

-- Separação
CREATE INDEX idx_picking_orders_status ON picking_orders(status);
CREATE INDEX idx_picking_orders_user ON picking_orders(atribuido_usuario_id);
CREATE INDEX idx_picking_orders_wave ON picking_orders(onda_id);

-- Expedição
CREATE INDEX idx_shipments_status ON shipments(status);
CREATE INDEX idx_shipments_warehouse ON shipments(armazem_id);
CREATE INDEX idx_shipments_carrier ON shipments(transportadora_id);

-- Busca full-text
CREATE INDEX idx_skus_search ON skus USING GIN(to_tsvector('portuguese', nome || ' ' || descricao));
CREATE INDEX idx_orders_search ON orders USING GIN(to_tsvector('portuguese', numero_pedido));
```

---

## 6. Estratégia de Particionamento

Para tabelas grandes (> 500 milhões de registros), implementar particionamento:

```sql
-- Particionamento por data (inventory_transactions)
CREATE TABLE inventory_transactions_2024_q1 PARTITION OF inventory_transactions
    FOR VALUES FROM ('2024-01-01') TO ('2024-04-01');

CREATE TABLE inventory_transactions_2024_q2 PARTITION OF inventory_transactions
    FOR VALUES FROM ('2024-04-01') TO ('2024-07-01');

-- Particionamento por tenant (para multi-tenancy mais severo)
-- (Alternativa: usar Row Level Security em vez de particionamento físico)
```

---

## 7. Fluxos Multi-Armazém e Multi-Estoque

### 7.1 Arquitetura Multi-Armazém

O sistema foi projetado para suportar múltiplos armazéns com as seguintes características:

#### Hierarquia Organizacional
```
TENANT (Organização)
  └─ WAREHOUSE (Armazém físico)
      └─ STOCK (Estoque - pode ser dedicado ou compartilhado)
          └─ LOCATION (Posição física - corredor, rack, prateleira, bin)
              └─ INVENTORY_MASTER (Produto em estoque)
```

#### Controle de Acesso Multi-Armazém

1. **Usuários e Armazéns:**
   - Relacionamento N:N através de `user_warehouses`
   - Um usuário pode ter acesso a múltiplos armazéns
   - Cada vínculo pode ter status independente (ACTIVE/INACTIVE)

2. **Roles e Permissões por Armazém:**
   - Roles definidas globalmente em `roles` com `base_permissions`
   - Atribuição específica por armazém em `user_warehouse_roles`
   - Permissões podem ser sobrescritas ou restringidas por armazém
   - Um usuário pode ter diferentes roles em diferentes armazéns

3. **Exemplo de Atribuição:**
   ```
   Usuário: João Silva
   ├─ Armazém São Paulo
   │   └─ Role: WAREHOUSE_SUPERVISOR (com permissões completas)
   └─ Armazém Rio de Janeiro
       └─ Role: PICKING_OPERATOR (apenas leitura e picking)
   ```

### 7.2 Estoques Dedicados vs Compartilhados

#### Estoque Dedicado (DEDICATED)
- Atribuído a um único cliente (`customer_id` NOT NULL)
- Isolamento completo de produtos
- Usado para clientes que requerem segregação física
- Exemplo: Cliente premium com produtos de alto valor

#### Estoque Compartilhado (SHARED)
- Pode armazenar produtos de múltiplos clientes (`customer_id` IS NULL)
- Maior eficiência de espaço
- Requer controle rigoroso de rastreabilidade
- Usado para operações de fulfillment

#### Estoque de Quarentena (QUARANTINE)
- Para produtos em análise de qualidade
- Bloqueado para operações normais até aprovação
- Pode ser por cliente ou compartilhado

#### Estoque de Devoluções (RETURNS)
- Produtos devolvidos aguardando inspeção
- Workflow separado de entrada normal

### 7.3 Fluxos de Movimentação

#### 1. Transferência Interna (TRANSFER)
```
Movimentação dentro do mesmo estoque
Stock A → Location 1 → Location 2 (mesmo Stock A)
```
- Tipo: `inventory_transactions.transaction_type = 'TRANSFER'`
- Usa: `location_from_id` e `location_to_id`
- Não requer aprovação especial

#### 2. Transferência entre Estoques (STOCK_TRANSFER)
```
Movimentação entre estoques do mesmo armazém
Warehouse 1 → Stock A → Stock B (mesmo Warehouse 1)
```
- Tipo: `stock_transfers` com `transfer_type = 'INTER_STOCK'`
- Requer: Aprovação do supervisor
- Gera: Transações em `inventory_transactions`
- Atualiza: Capacidades de ambos os estoques

#### 3. Transferência entre Armazéns (WAREHOUSE_TRANSFER)
```
Movimentação entre armazéns diferentes
Warehouse SP → Stock A → Warehouse RJ → Stock B
```
- Tipo: `stock_transfers` com `transfer_type = 'INTER_WAREHOUSE'`
- Requer: Aprovação de ambos os gestores de armazém
- Processo:
  1. Criação do pedido de transferência (DRAFT)
  2. Aprovação (PENDING_APPROVAL → APPROVED)
  3. Picking no armazém origem (IN_TRANSIT)
  4. Transporte físico
  5. Recebimento no armazém destino (COMPLETED)
- Gera: ASN automático no armazém destino

#### 4. Integração com Processos Normais

**Recebimento (Inbound):**
```sql
-- ASN especifica o estoque de destino
inbound_asn.armazem_id → Armazém de destino
receiving_operations.localizacao_final_id → Location dentro do Stock

-- Após putaway, atualiza inventory_master
INSERT INTO inventory_master (armazem_id, estoque_id, localizacao_id, ...)
```

**Separação (Outbound):**
```sql
-- Pedido determina de qual estoque retirar
orders.armazem_id → Armazém fonte

-- Sistema seleciona estoque baseado em:
-- 1. Disponibilidade
-- 2. FIFO/FEFO
-- 3. Proximidade da área de expedição
-- 4. Cliente (se estoque dedicado)

-- Linhas de separação referenciam localizações dentro do estoque
picking_lines.localizacao_id → Location do Stock
```

**Conferência e Alocação:**
```sql
-- Durante recebimento, sistema sugere estoque baseado em:
-- 1. Cliente do produto (se estoque dedicado)
-- 2. Tipo de produto (categoria)
-- 3. Requisitos especiais (temperatura, materiais perigosos)
-- 4. Disponibilidade de espaço
-- 5. Estratégia de putaway

-- Validações:
-- - stock.permite_produtos_mistos
-- - stock.categorias_produto_permitidas
-- - stock.temperatura_controlada
-- - location.peso_maximo, volume_maximo
```

### 7.4 Queries Úteis

#### Verificar acesso de usuário a armazéns
```sql
SELECT
    u.nome_completo,
    w.nome AS nome_armazem,
    r.nome AS nome_role,
    uwr.permissoes_especificas_armazem
FROM users u
JOIN user_warehouses uw ON u.id = uw.usuario_id
JOIN warehouses w ON uw.armazem_id = w.id
LEFT JOIN user_warehouse_roles uwr ON uw.id = uwr.usuario_armazem_id
LEFT JOIN roles r ON uwr.role_id = r.id
WHERE u.id = '{user_id}'
  AND uw.status = 'ATIVO'
  AND uwr.status = 'ATIVO';
```

#### Inventário consolidado por armazém e estoque
```sql
SELECT
    w.nome AS armazem,
    s.nome AS estoque,
    s.tipo_estoque,
    c.nome AS cliente,
    COUNT(DISTINCT im.sku_id) AS total_skus,
    SUM(im.quantidade_em_maos) AS quantidade_total,
    SUM(im.quantidade_disponivel) AS quantidade_disponivel,
    s.posicoes_ocupadas_atual,
    s.capacidade_total_posicoes,
    s.percentual_utilizacao
FROM warehouses w
JOIN stocks s ON w.id = s.armazem_id
LEFT JOIN customers c ON s.cliente_id = c.id
LEFT JOIN inventory_master im ON s.id = im.estoque_id
WHERE w.tenant_id = '{tenant_id}'
  AND s.status = 'ATIVO'
GROUP BY w.id, s.id, c.id;
```

#### Transferências pendentes entre armazéns
```sql
SELECT
    st.numero_transferencia,
    st.tipo_transferencia,
    wf.nome AS armazem_origem,
    sf.nome AS estoque_origem,
    wt.nome AS armazem_destino,
    st2.nome AS estoque_destino,
    st.status,
    st.quantidade_total,
    st.quantidade_recebida,
    st.data_agendada
FROM stock_transfers st
JOIN warehouses wf ON st.armazem_origem_id = wf.id
JOIN stocks sf ON st.estoque_origem_id = sf.id
JOIN warehouses wt ON st.armazem_destino_id = wt.id
JOIN stocks st2 ON st.estoque_destino_id = st2.id
WHERE st.tenant_id = '{tenant_id}'
  AND st.status IN ('APROVADO', 'EM_TRANSITO', 'PARCIALMENTE_RECEBIDO')
ORDER BY st.data_agendada;
```

---

## 8. Backup e Disaster Recovery

### 8.1 Estratégia de Backup

- **Full Backup:** Diário (00:00 UTC)
- **Incremental:** A cada 6 horas
- **Retenção:** 30 dias em cold storage
- **Point-in-Time Recovery:** Até 7 dias

### 8.2 Replicação

- Read replicas em 3 data centers
- Sincronização em tempo real (RPO < 1 segundo)
- Failover automático com RTO < 5 minutos

---

---

## 9. Resumo das Alterações (Multi-Armazém e Multi-Estoque)

### Principais Mudanças Implementadas:

1. **Sistema Multi-Armazém Completo:**
   - Usuários podem ter acesso a múltiplos armazéns via `user_warehouses`
   - Roles e permissões específicas por armazém via `user_warehouse_roles`
   - Controle granular de acesso por armazém e módulo

2. **Camada de Estoques (Stocks):**
   - Nova tabela `stocks` entre `warehouses` e `locations`
   - Suporte para estoques dedicados (cliente único) e compartilhados
   - Tipos especiais: QUARANTINE, RETURNS
   - Controle de capacidade, temperatura, e restrições por estoque

3. **Movimentações Entre Estoques e Armazéns:**
   - Tabelas `stock_transfers` e `stock_transfer_lines`
   - Suporte completo para transferências inter-estoque e inter-armazém
   - Workflow de aprovação e rastreamento
   - Integração com processos de picking e recebimento

4. **Atualizações nas Tabelas Existentes:**
   - `users`: Removido campo `warehouse_ids` (agora usa tabela intermediária)
   - `roles`: Campo `permissions` renomeado para `base_permissions`
   - `locations`: Adicionado campo `stock_id`
   - `inventory_master`: Adicionado campo `stock_id`
   - `inventory_transactions`: Novos campos para rastreamento de origem/destino

5. **Novas Tabelas:**
   - `user_warehouses`: Relacionamento usuário-armazém
   - `user_warehouse_roles`: Roles e permissões por armazém
   - `stocks`: Estoques dentro de armazéns
   - `stock_transfers`: Transferências entre estoques/armazéns
   - `stock_transfer_lines`: Linhas das transferências

### Impactos na Aplicação:

- **Autenticação/Autorização:** Verificar permissões por armazém no contexto atual
- **Seleção de Estoque:** Lógica para determinar estoque na alocação de produtos
- **Transferências:** Novos endpoints e workflows para movimentação
- **Relatórios:** Agregações por warehouse → stock → location
- **UI:** Seletor de armazém no contexto do usuário

---

**Documento Versão:** 2.0
**Status:** Design Atualizado - Multi-Armazém e Multi-Estoque
**Última Atualização:** 2025-11-18
**Próximo Passo:** Revisão e implementação

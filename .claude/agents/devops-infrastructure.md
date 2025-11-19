# ⚙️ DevOps & Infrastructure Agent

## Especialização
CI/CD, deployment, containerização, Kubernetes, infraestrutura como código, monitoring, logging e operações.

## Responsabilidades Principais

### 1. **CI/CD Pipeline**
- GitHub Actions / GitLab CI configuração
- Build automation
- Testing automation (unit, integration, E2E)
- Artifact management
- Deploy automation

### 2. **Containerização**
- Docker setup
- Image optimization
- Registry management (DockerHub, ECR)
- Container security

### 3. **Kubernetes (K8s)**
- Deployment manifests
- Service/Ingress configuration
- StatefulSets para dados
- ConfigMaps e Secrets
- Resource quotas e limits
- Auto-scaling (HPA)

### 4. **Deployment Strategies**
- Blue-green deployment
- Canary releases
- Rolling updates
- Rollback procedures
- Zero-downtime migrations

### 5. **Infrastructure as Code (IaC)**
- Terraform para AWS/Azure/GCP
- Provisioning automático
- Environment configuration
- Disaster recovery

### 6. **Monitoring & Observability**
- Prometheus para métricas
- Grafana para dashboards
- ELK Stack (Elasticsearch, Logstash, Kibana)
- Distributed tracing (Jaeger)
- Alerting

### 7. **Backup & Disaster Recovery**
- Backup strategy
- RTO/RPO targets
- Restore procedures
- Testing de DR

## Contexto Documentado

### Documentos Principais (DEVE ESTUDAR)
1. **11_DEPLOYMENT_DEVOPS.md**
   - Estratégia de deployment (staging, production, canary)
   - CI/CD pipeline (GitLab CI, GitHub Actions)
   - Kubernetes deployment (namespaces, deployments, services)
   - Blue-green deployment strategy
   - Rollback strategy
   - Infrastructure as Code (Terraform)
   - Container registry setup
   - Logging e monitoring (ELK, Prometheus, Grafana)
   - Runbooks para operações
   - Disaster recovery procedures

2. **10_PERFORMANCE_ESCALABILIDADE.md**
   - Escalabilidade horizontal
   - Auto-scaling triggers e limites
   - Performance targets
   - Disaster recovery (RTO/RPO)

### Documentos Secundários (REFERÊNCIA)
- 03_ARQUITETURA_SISTEMA.md - Stack de infra
- 04_DESIGN_BANCO_DADOS.md - Backup strategy
- 09_SEGURANCA.md - Secrets management

## CI/CD Pipeline

### GitHub Actions
```yaml
name: Build and Deploy

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    # Build backend
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.0'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Run unit tests
      run: dotnet test --no-build --verbosity normal --filter "Category=Unit"

    - name: Run integration tests
      run: dotnet test --no-build --verbosity normal --filter "Category=Integration"

    # Build frontend
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'

    - name: Install dependencies
      run: npm ci
      working-directory: ./frontend

    - name: Build frontend
      run: npm run build
      working-directory: ./frontend

    - name: Run frontend tests
      run: npm test
      working-directory: ./frontend

    # Docker build
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v2

    - name: Login to DockerHub
      uses: docker/login-action@v2
      with:
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}

    - name: Build and push Docker image
      uses: docker/build-push-action@v4
      with:
        context: .
        push: true
        tags: |
          myrepo/wms-api:latest
          myrepo/wms-api:${{ github.sha }}

    # Deploy
    - name: Deploy to Kubernetes
      if: github.ref == 'refs/heads/main'
      run: |
        kubectl set image deployment/wms-api \
          wms-api=myrepo/wms-api:${{ github.sha }} \
          -n production
```

### GitLab CI
```yaml
stages:
  - build
  - test
  - docker
  - deploy

build_backend:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:10.0
  script:
    - dotnet restore
    - dotnet build --configuration Release
  artifacts:
    paths:
      - backend/src/WMS.API/bin/Release/

test_backend:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:10.0
  script:
    - dotnet test --verbosity normal
  coverage: '/^[^(]*\([^)]*\)\s*:\s*\d+%$/'

test_frontend:
  stage: test
  image: node:18
  script:
    - cd frontend
    - npm ci
    - npm test
    - npm run build

docker_build:
  stage: docker
  image: docker:latest
  services:
    - docker:dind
  script:
    - docker build -t $CI_REGISTRY_IMAGE:$CI_COMMIT_SHA .
    - docker push $CI_REGISTRY_IMAGE:$CI_COMMIT_SHA
  only:
    - main

deploy_production:
  stage: deploy
  image: bitnami/kubectl:latest
  script:
    - kubectl set image deployment/wms-api wms-api=$CI_REGISTRY_IMAGE:$CI_COMMIT_SHA -n production
    - kubectl rollout status deployment/wms-api -n production
  only:
    - main
```

## Dockerfile

```dockerfile
# Build stage (multistage)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/src/WMS.API/WMS.API.csproj", "WMS.API/"]
COPY ["backend/src/WMS.Domain/WMS.Domain.csproj", "WMS.Domain/"]
COPY ["backend/src/WMS.Application/WMS.Application.csproj", "WMS.Application/"]
COPY ["backend/src/WMS.Infrastructure/WMS.Infrastructure.csproj", "WMS.Infrastructure/"]
COPY ["backend/src/WMS.Shared/WMS.Shared.csproj", "WMS.Shared/"]

RUN dotnet restore "WMS.API/WMS.API.csproj"

COPY . .
RUN dotnet build "WMS.API/WMS.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WMS.API/WMS.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Segurança
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WMS.API.dll"]
```

## Kubernetes Deployment

### Namespaces
```yaml
---
apiVersion: v1
kind: Namespace
metadata:
  name: wms-development

---
apiVersion: v1
kind: Namespace
metadata:
  name: wms-staging

---
apiVersion: v1
kind: Namespace
metadata:
  name: wms-production
```

### Deployment (API)
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: wms-api
  namespace: wms-production
  labels:
    app: wms-api
spec:
  replicas: 3  # High availability
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0  # Zero-downtime
  selector:
    matchLabels:
      app: wms-api
  template:
    metadata:
      labels:
        app: wms-api
      annotations:
        prometheus.io/scrape: "true"
        prometheus.io/port: "8080"
        prometheus.io/path: "/metrics"
    spec:
      containers:
      - name: wms-api
        image: myrepo/wms-api:latest
        imagePullPolicy: IfNotPresent
        ports:
        - containerPort: 8080
          name: http
          protocol: TCP
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: DATABASE_HOST
          valueFrom:
            configMapKeyRef:
              name: db-config
              key: host
        - name: DATABASE_PASSWORD
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: password
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
```

### Service
```yaml
apiVersion: v1
kind: Service
metadata:
  name: wms-api-service
  namespace: wms-production
spec:
  type: LoadBalancer
  selector:
    app: wms-api
  ports:
  - port: 80
    targetPort: 8080
    protocol: TCP
    name: http
```

### Ingress
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: wms-ingress
  namespace: wms-production
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - api.wms-enterprise.com
    secretName: wms-tls-cert
  rules:
  - host: api.wms-enterprise.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: wms-api-service
            port:
              number: 80
```

### HPA (Horizontal Pod Autoscaler)
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: wms-api-hpa
  namespace: wms-production
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: wms-api
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

## Terraform (Infrastructure as Code)

```hcl
# AWS
provider "aws" {
  region = var.aws_region
}

# VPC
resource "aws_vpc" "main" {
  cidr_block = "10.0.0.0/16"

  tags = {
    Name = "wms-vpc"
  }
}

# RDS (PostgreSQL)
resource "aws_db_instance" "postgres" {
  identifier     = "wms-postgres"
  engine         = "postgres"
  engine_version = "14.7"
  instance_class = "db.t3.medium"
  allocated_storage = 100
  storage_encrypted = true

  db_name  = "wms"
  username = var.db_username
  password = random_password.db_password.result

  backup_retention_period = 30
  multi_az = true

  tags = {
    Name = "wms-postgres"
  }
}

# ElastiCache (Redis)
resource "aws_elasticache_cluster" "redis" {
  cluster_id           = "wms-redis"
  engine               = "redis"
  node_type            = "cache.t3.medium"
  num_cache_nodes      = 3
  parameter_group_name = "default.redis7"
  engine_version       = "7.0"
  port                 = 6379

  tags = {
    Name = "wms-redis"
  }
}

# EKS (Kubernetes)
resource "aws_eks_cluster" "main" {
  name            = "wms-cluster"
  version         = "1.27"
  role_arn        = aws_iam_role.eks_service_role.arn

  vpc_config {
    subnet_ids = var.subnet_ids
  }

  depends_on = [
    aws_iam_role_policy_attachment.AmazonEKSServiceRolePolicy,
  ]

  tags = {
    Name = "wms-cluster"
  }
}

# Outputs
output "rds_endpoint" {
  value       = aws_db_instance.postgres.endpoint
  sensitive   = false
}

output "redis_endpoint" {
  value       = aws_elasticache_cluster.redis.cache_nodes[0].address
  sensitive   = false
}

output "eks_endpoint" {
  value       = aws_eks_cluster.main.endpoint
  sensitive   = false
}
```

## Monitoring & Logging

### Prometheus Scrape Config
```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'wms-api'
    kubernetes_sd_configs:
      - role: pod
        namespaces:
          names:
            - wms-production
    relabel_configs:
      - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_scrape]
        action: keep
        regex: true
      - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_path]
        action: replace
        target_label: __metrics_path__
        regex: (.+)
      - source_labels: [__address__, __meta_kubernetes_pod_annotation_prometheus_io_port]
        action: replace
        regex: ([^:]+)(?::\d+)?;(\d+)
        replacement: $1:$2
        target_label: __address__
```

### Grafana Dashboard
```json
{
  "dashboard": {
    "title": "WMS Enterprise - API Metrics",
    "panels": [
      {
        "title": "Request Rate",
        "targets": [
          {
            "expr": "rate(http_requests_total[5m])"
          }
        ]
      },
      {
        "title": "Error Rate",
        "targets": [
          {
            "expr": "rate(http_requests_total{status=~'5..'}[5m])"
          }
        ]
      },
      {
        "title": "Latency (P95)",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))"
          }
        ]
      }
    ]
  }
}
```

### ELK Stack
```yaml
version: '3.8'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.0.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    ports:
      - "9200:9200"

  logstash:
    image: docker.elastic.co/logstash/logstash:8.0.0
    volumes:
      - ./logstash.conf:/usr/share/logstash/pipeline/logstash.conf
    ports:
      - "5000:5000"
    depends_on:
      - elasticsearch

  kibana:
    image: docker.elastic.co/kibana/kibana:8.0.0
    ports:
      - "5601:5601"
    depends_on:
      - elasticsearch
```

## Backup & Disaster Recovery

### RDS Backup Strategy
```
Daily Automated Backups:
├─ Automated snapshots diários
├─ Retenção: 30 dias
└─ Multi-AZ replication

Monthly Manual Snapshots:
├─ Snapshot no último dia de cada mês
└─ Retenção: 90 dias

Recovery:
├─ RTO: < 1 hora
├─ RPO: < 15 minutos
└─ Tested quarterly
```

### Restore Procedure
```bash
#!/bin/bash

# 1. Listar snapshots disponíveis
aws rds describe-db-snapshots \
  --db-instance-identifier wms-postgres \
  --query "DBSnapshots[].DBSnapshotIdentifier"

# 2. Restaurar de snapshot
aws rds restore-db-instance-from-db-snapshot \
  --db-instance-identifier wms-postgres-restore \
  --db-snapshot-identifier snapshot-id \
  --publicly-accessible false

# 3. Aguardar disponibilidade
aws rds wait db-instance-available \
  --db-instance-identifier wms-postgres-restore

# 4. Validar dados
psql -h restored-instance-endpoint -U admin -d wms \
  -c "SELECT COUNT(*) FROM orders;"

# 5. Update connection strings (se necessário)
kubectl set env deployment/wms-api \
  DATABASE_HOST=restored-instance-endpoint \
  -n production

# 6. Teste de aplicação
curl https://api.wms-enterprise.com/health/ready
```

## Exemplos de Prompts

```
1. "Configure um CI/CD pipeline completo com GitHub Actions.
    Deve incluir testes, build, e deploy automático."

2. "Crie um Dockerfile otimizado para a API.
    Qual é o tamanho mínimo aceitável?"

3. "Design o Kubernetes deployment para 99.95% uptime.
    Quantas réplicas? Como fazer rolling updates?"

4. "Implemente HPA (autoscaling) com CPU e memory.
    Quais são os triggers recomendados?"

5. "Qual é a estratégia de backup e disaster recovery?
    RTO < 1h, RPO < 15min?"

6. "Configure monitoramento com Prometheus e Grafana.
    Quais métricas são críticas?"

7. "Como implementar blue-green deployment?"

8. "Crie uma estratégia de secrets management com Vault/AWS Secrets Manager."
```

## Fluxo de Trabalho Típico

### 1. **Setup Inicial**
- Provisionar infraestrutura (Terraform)
- Setup Kubernetes
- Configurar registries

### 2. **CI/CD**
- Criar pipeline
- Configurar testes
- Automatizar builds

### 3. **Deployment**
- Deploy para staging
- Deploy para produção (canary)
- Monitorar

### 4. **Operações**
- Alerting
- Debugging
- Scaling
- Maintenance

### 5. **DR Testing**
- Testar backups
- Testar restore
- Validar RTO/RPO

## Checklist de Produção

- [ ] CI/CD pipeline automatizado?
- [ ] Testes rodam em pipeline?
- [ ] Docker images pequenas (< 500MB)?
- [ ] Kubernetes deployments com 3+ replicas?
- [ ] Health checks (liveness + readiness)?
- [ ] Resource limits definidos?
- [ ] HPA configurado?
- [ ] Monitoring/alerting ativo?
- [ ] Logging centralizado?
- [ ] Backup automático?
- [ ] Secrets seguros?
- [ ] TLS/HTTPS habilitado?
- [ ] CORS configurado?

## Integração com Outros Agentes

```
DevOps & Infrastructure Agent
    ↓
    ├─→ Backend Architect (deployment considerations)
    ├─→ Security & Compliance (secrets, security headers)
    ├─→ Database Engineer (backup/restore)
    └─→ Todos (suporta todos os agentes)
```

## Responsabilidades Diárias

- Monitorar pipelines e deployments
- Responder a alertas
- Manter infra atualizada
- Otimizar performance
- Responder dúvidas de ops

## Conhecimento Esperado

- Docker e containerização
- Kubernetes e orquestração
- CI/CD (GitHub Actions, GitLab CI)
- Infrastructure as Code (Terraform)
- AWS / Azure / GCP
- Prometheus / Grafana
- ELK Stack / Loki
- Database administration
- Networking e firewalls
- Bash/scripting

---

**Versão:** 1.0
**Criado:** Novembro 2025
**Status:** Ativo
**Próxima Revisão:** Após Sprint 2 (primeira release)

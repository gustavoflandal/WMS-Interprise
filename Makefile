.PHONY: help install build test clean docker-up docker-down backend-run frontend-run

# Colors for output
GREEN  := $(shell tput -Txterm setaf 2)
YELLOW := $(shell tput -Txterm setaf 3)
WHITE  := $(shell tput -Txterm setaf 7)
RESET  := $(shell tput -Txterm sgr0)

help: ## Mostra esta mensagem de ajuda
	@echo ''
	@echo 'Uso:'
	@echo '  ${YELLOW}make${RESET} ${GREEN}<target>${RESET}'
	@echo ''
	@echo 'Targets:'
	@awk 'BEGIN {FS = ":.*?## "} { \
		if (/^[a-zA-Z_-]+:.*?##.*$$/) {printf "    ${YELLOW}%-20s${GREEN}%s${RESET}\n", $$1, $$2} \
		else if (/^## .*$$/) {printf "  ${WHITE}%s${RESET}\n", substr($$1,4)} \
		}' $(MAKEFILE_LIST)

## Instalação
install: install-backend install-frontend ## Instala todas as dependências

install-backend: ## Instala dependências do backend
	@echo "${GREEN}Instalando dependências do backend...${RESET}"
	cd backend/src && dotnet restore

install-frontend: ## Instala dependências do frontend
	@echo "${GREEN}Instalando dependências do frontend...${RESET}"
	cd frontend && npm install

## Build
build: build-backend build-frontend ## Build de todo o projeto

build-backend: ## Build do backend
	@echo "${GREEN}Building backend...${RESET}"
	cd backend/src && dotnet build --configuration Release

build-frontend: ## Build do frontend
	@echo "${GREEN}Building frontend...${RESET}"
	cd frontend && npm run build

## Testes
test: test-backend test-frontend ## Executa todos os testes

test-backend: ## Executa testes do backend
	@echo "${GREEN}Executando testes do backend...${RESET}"
	cd backend && dotnet test --configuration Release

test-frontend: ## Executa testes do frontend
	@echo "${GREEN}Executando testes do frontend...${RESET}"
	cd frontend && npm run test

test-coverage: ## Gera relatório de cobertura
	@echo "${GREEN}Gerando relatório de cobertura...${RESET}"
	cd backend && dotnet test --collect:"XPlat Code Coverage"
	cd frontend && npm run test:coverage

## Desenvolvimento
dev: ## Inicia ambiente de desenvolvimento
	@echo "${GREEN}Iniciando ambiente de desenvolvimento...${RESET}"
	$(MAKE) -j2 backend-run frontend-run

backend-run: ## Executa backend em modo desenvolvimento
	@echo "${GREEN}Iniciando backend...${RESET}"
	cd backend/src/WMS.API && dotnet run

frontend-run: ## Executa frontend em modo desenvolvimento
	@echo "${GREEN}Iniciando frontend...${RESET}"
	cd frontend && npm run dev

## Docker
docker-up: ## Sobe todos os containers
	@echo "${GREEN}Iniciando containers Docker...${RESET}"
	docker-compose up -d

docker-down: ## Para todos os containers
	@echo "${GREEN}Parando containers Docker...${RESET}"
	docker-compose down

docker-logs: ## Mostra logs dos containers
	docker-compose logs -f

docker-clean: ## Remove containers, volumes e imagens
	@echo "${YELLOW}Limpando containers, volumes e imagens...${RESET}"
	docker-compose down -v --rmi all

docker-rebuild: ## Rebuild e restart dos containers
	@echo "${GREEN}Reconstruindo containers...${RESET}"
	docker-compose down
	docker-compose build --no-cache
	docker-compose up -d

## Database
db-migrate: ## Executa migrações do banco de dados
	@echo "${GREEN}Executando migrações...${RESET}"
	cd backend/src/WMS.Infrastructure && dotnet ef database update

db-migration-add: ## Cria nova migração (use NAME=nome_da_migracao)
	@echo "${GREEN}Criando nova migração: $(NAME)${RESET}"
	cd backend/src/WMS.Infrastructure && dotnet ef migrations add $(NAME)

db-seed: ## Popula banco com dados iniciais
	@echo "${GREEN}Populando banco de dados...${RESET}"
	# Adicionar comando de seed aqui

## Linting e Formatação
lint: lint-backend lint-frontend ## Executa lint em todo o código

lint-backend: ## Lint do backend
	@echo "${GREEN}Executando lint do backend...${RESET}"
	cd backend && dotnet format --verify-no-changes

lint-frontend: ## Lint do frontend
	@echo "${GREEN}Executando lint do frontend...${RESET}"
	cd frontend && npm run lint

format: format-backend format-frontend ## Formata todo o código

format-backend: ## Formata código do backend
	@echo "${GREEN}Formatando código do backend...${RESET}"
	cd backend && dotnet format

format-frontend: ## Formata código do frontend
	@echo "${GREEN}Formatando código do frontend...${RESET}"
	cd frontend && npm run format

## Limpeza
clean: clean-backend clean-frontend ## Limpa arquivos de build

clean-backend: ## Limpa build do backend
	@echo "${GREEN}Limpando backend...${RESET}"
	cd backend && dotnet clean
	find backend -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
	find backend -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true

clean-frontend: ## Limpa build do frontend
	@echo "${GREEN}Limpando frontend...${RESET}"
	rm -rf frontend/dist
	rm -rf frontend/node_modules
	rm -rf frontend/coverage

## Informações
info: ## Mostra informações do ambiente
	@echo "${GREEN}Informações do ambiente:${RESET}"
	@echo "Node version: $$(node --version)"
	@echo "NPM version: $$(npm --version)"
	@echo ".NET version: $$(dotnet --version)"
	@echo "Docker version: $$(docker --version)"
	@echo "Docker Compose version: $$(docker-compose --version)"

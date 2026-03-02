.PHONY: build prepare run test publish migration update-db clean docker-up docker-down help

WEBAPI_PROJECT=src/Itau.AutoInvest.WebApi/Itau.AutoInvest.WebApi.csproj
INFRA_PROJECT=src/Itau.AutoInvest.Infrastructure/Itau.AutoInvest.Infrastructure.csproj
SOLUTION=Itau.AutoInvest.slnx

all: build

build:
	dotnet build $(SOLUTION)

prepare:
	dotnet tool restore

run:
	dotnet run --project $(WEBAPI_PROJECT)

test:
	dotnet test $(SOLUTION)

test-unit:
	dotnet test tests/Itau.AutoInvest.Tests/Itau.AutoInvest.Tests.csproj

test-int:
	dotnet test tests/Itau.AutoInvest.Tests.Integration/Itau.AutoInvest.Tests.Integration.csproj

publish:
	dotnet publish $(WEBAPI_PROJECT) -c Release -o ./publish

clean:
	dotnet clean $(SOLUTION)
	rm -rf ./publish
	find . -type d -name "obj" -exec rm -rf {} +
	find . -type d -name "bin" -exec rm -rf {} +

# Usage: make migration name=NomeDaMinhaMigration
migration:
	@if [ -z "$(name)" ]; then echo "Erro: Forneça o nome da migration. Ex: make migration name=InitialCreate"; exit 1; fi
	dotnet ef migrations add $(name) --project $(INFRA_PROJECT) --startup-project $(WEBAPI_PROJECT)

update-db:
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(WEBAPI_PROJECT)

docker-up:
	docker compose up -d

docker-down:
	docker compose down

help:
	@echo "Comandos disponíveis:"
	@echo "  make build          - Compila a solução completa"
	@echo "  make prepare        - Instala as DotNet Tools necessarias"
	@echo "  make run            - Executa o projeto WebApi"
	@echo "  make test           - Executa todos os testes (Unitários e Integração)"
	@echo "  make test-unit      - Executa apenas os testes unitários"
	@echo "  make test-int       - Executa apenas os testes de integração"
	@echo "  make publish        - Publica o projeto WebApi em modo Release"
	@echo "  make migration name=NAME - Cria uma nova migration do EF"
	@echo "  make update-db      - Aplica as migrations no banco de dados"
	@echo "  make clean          - Limpa artefatos de build (bin/obj/publish)"
	@echo "  make docker-up      - Sobe a infraestrutura no Docker"
	@echo "  make docker-down    - Para a infraestrutura no Docker"
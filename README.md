# Itaú AutoInvest - Sistema de Compra Programada de Ações

Este projeto consiste num desafio para a vaga de Engenheiro de Software da Itaú Corretora que permite aos clientes investir mensalmente em uma carteira recomendada de 5 ações (Top Five). O sistema automatiza a compra consolidada na conta master, a distribuição proporcional para as contas dos clientes e o rebalanceamento da carteira.

## Tecnologias Utilizadas

- Backend: .NET Core 10.0 (C#)
- Banco de Dados: MySQL 8.0
- Mensageria: Apache Kafka
- Conteinerização: Docker e Docker Compose
- Documentação: Swagger / OpenAPI
- Testes: xUnit, Moq

## Arquitetura e Decisões Técnicas

O sistema foi desenvolvido seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD). A estrutura esta dividida em:

- Itau.AutoInvest.Domain: Contem as entidades de negócio, enums, exceções e objetos de valor.
- Itau.AutoInvest.Application: Contem os casos de uso, interfaces de abstração, DTOs e jobs agendados.
- Itau.AutoInvest.Infrastructure: Implementa o acesso a dados (EF Core), repositórios, mapeadores e integração com Kafka.
- Itau.AutoInvest.WebApi: Porta de entrada do sistema com os controllers REST e configurações de injeção de dependência.

## Pre-requisitos

- .NET 9.0 SDK
- Docker e Docker Compose
- Ferramenta Make (opcional, mas recomendado para facilitar os comandos)

## Como Executar o Projeto

1. Clone o repositório.
2. Na raiz do projeto, suba a infraestrutura necessaria (MySQL e Kafka):
   ```bash
   make docker-up
   ```
   Ou use o comando: `docker compose up -d`

3. Restaure as ferramentas e dependências:
   ```bash
   make prepare
   ```
   Ou use o comando: `dotnet tool restore`

4. Aplique as migrations para criar o banco de dados:
   ```bash
   make update-db
   ```
   Ou use o comando: `dotnet ef database update --startup-project src/Itau.AutoInvest.WebApi --project src/Itau.AutoInvest.Infrastructure`

5. Execute a aplicação:
   ```bash
   make run
   ```
   Ou use o comando: `dotnet run --project src/Itau.AutoInvest.WebApi`

O Swagger estará disponível em `http://localhost:8080/` para consulta da documentação e testes dos endpoints.
Se preferir, na pasta [.http](./.http) contem arquivos .http para teste dos endpoints.

## Como Executar os Testes

- Para os testes de integração, será necessário ter as dependências externas (como o Kafka) rodando.

Para rodar todos os testes (unitários e integração):
```bash
make test
```

Para rodar apenas testes unitários:
```bash
make test-unit
```

Para rodar apenas testes de integração:
```bash
make test-int
```

## Estrutura de Pastas

- `src/`: Codigo-fonte do sistema.
- `tests/`: Testes unitarios e de integração.
- `docs/`: Documentacao do desafio e requisitos.
- `cotacoes/`: Pasta onde devem ser colocados os arquivos TXT de cotacoes da B3.

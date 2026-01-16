# Seguradora Monorepo

Ecossistema de microsserviços para gestão de seguros, composto pelo **PropostaService** e **ContratacaoService**. A solução utiliza comunicação assíncrona baseada em eventos para garantir o desacoplamento e a consistência eventual entre os domínios.

## Contexto e Fluxo de Operação

O ecossistema opera sob o seguinte fluxo de integração técnica:

1.  **Entrada**: Uma proposta de seguro é enviada para o `PropostaService` via endpoint `POST /api/v1/propostas`.
2.  **Análise**: A análise automatizada de dados poderá ser implementada por um microsserviço futuro de motor de regras.
3.  **Aprovação**: Para que a proposta siga o fluxo, deve ser utilizado o endpoint `POST /api/v1/propostas/{id}/aprovar`.
4.  **Evento**: Ao ser marcada como aprovada, o `PropostaService` publica um evento `PropostaAprovadaEvent` no RabbitMQ.
5.  **Consumo**: O `ContratacaoService` consome o evento, valida a integridade da proposta via integração com API REST e persiste a nova contratação.
6.  **Resultado**: A contratação é efetivada e disponibilizada para consulta via endpoint `GET /api/v1/contratacoes/{id}` ou `GET /api/v1/contratacoes/proposta/{propostaId}`.


## Pré-requisitos

- **Docker** e **Docker Compose**.
- **SDK .NET 8.0, PosgreSQL e RabbitMQ** (necessário apenas para build ou execução de testes fora de containers).

## Execução via Docker Compose (Recomendado)

A solução utiliza o recurso do Docker Compose para orquestrar a infraestrutura e os serviços em uma rede isolada. Para iniciar todo o ecossistema a partir da raiz do repositório, execute:

```bash
docker compose up -d --build
```

### Serviços Provisionados:

- Proposta API (Swagger): http://localhost:5000/swagger

- Contratação API (Swagger): http://localhost:5001/swagger

- PostgreSQL: Porta 5432 (Bancos: proposta_db e contratacao_db)

- RabbitMQ Management: http://localhost:15672 (Usuário: admin | Senha: admin)

As migrations do Entity Framework são executadas automaticamente no startup das aplicações através do Program.cs.

## Execução Local (Alternativa)

Caso seja necessário rodar os serviços individualmente via CLI:

Certifique-se de que os containers de infraestrutura (postgres e rabbitmq) estejam ativos.

Execute os comandos nas respectivas pastas:

```bash
dotnet run --project src/PropostaService.API/PropostaService.API.csproj
dotnet run --project src/ContratacaoService.API/ContratacaoService.API.csproj
```

## Instruções de Build e Testes

### Compilação

```bash
dotnet restore
dotnet build --configuration Release
```

### Testes Unitários

```bash
dotnet test tests/PropostaService.UnitTests
dotnet test tests/ContratacaoService.UnitTests
```

### Testes de Integração

Validam o pipeline de consumers, persistência e comunicação entre componentes.

```bash
dotnet test tests/PropostaService.IntegrationTests
dotnet test tests/ContratacaoService.IntegrationTests
```

## Detalhes de Integração Técnica

Comunicação Assíncrona: Realizada com MassTransit utilizando RabbitMQ.

Integração com API REST: O ContratacaoService consome o PropostaService para validação de integridade.

Persistência: PostgreSQL 16 com isolamento de base de dados por serviço.

Logs: Persistidos localmente nos volumes mapeados em ./proposta-logs e ./contratacao-logs.
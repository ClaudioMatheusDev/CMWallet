# CMWallet

Aplicação de controle financeiro pessoal: cadastre contas, categorias, transações e metas, e acompanhe o saldo e o resumo do mês. O back-end é uma API REST em **.NET 9** com **Entity Framework Core** e **SQL Server**; o front-end é uma SPA em **Angular 22** com renderização no servidor (SSR).

## Funcionalidades

- **Contas** (conta corrente, poupança e dinheiro) com saldo atual calculado a partir das transações pagas.
- **Categorias** de entrada e de saída.
- **Transações** com filtros por conta, categoria, tipo e período, e paginação.
- **Metas financeiras** vinculadas a uma conta, com valor alvo, valor atual e data.
- **Visão geral** do mês: total de entradas e saídas, resultado, valores pendentes e composição por categoria.
- Tema claro e escuro, navegação por teclado e textos em português do Brasil.

## Tecnologias

| Camada | Tecnologias |
| --- | --- |
| API | ASP.NET Core 9, Entity Framework Core 9, SQL Server, OpenAPI + Scalar, Health Checks |
| Front-end | Angular 22 (componentes standalone, signals, sem Zone.js, SSR), TypeScript, CSS próprio com tokens de design |
| Testes | xUnit e NSubstitute (back-end), Vitest (front-end) |

## Arquitetura

A solução segue uma separação em camadas, e as dependências apontam sempre para o domínio:

```text
CMWallet.API              Controllers, middleware de erros, CORS, OpenAPI
   │  ├── CMWallet.Application      Serviços, DTOs, exceções de negócio, interfaces dos repositórios
   │  │        └── CMWallet.Domain       Entidades e enums
   │  └── CMWallet.Infrastructure   DbContext, configurações do EF, migrations, repositórios
   │           ├── CMWallet.Application
   │           └── CMWallet.Domain
CMWallet.Tests            Testes dos serviços (referencia a Application)
frontend/                 SPA Angular
```

A camada `Application` declara as interfaces dos repositórios (`IContaRepository`, `ITransacaoRepository` e outras) e a `Infrastructure` as implementa, então as regras de negócio podem ser testadas sem banco de dados.

### Regras de negócio principais

- **Saldo da conta** = saldo inicial + entradas pagas − saídas pagas. O cálculo roda no banco, em uma subconsulta, sem carregar as transações em memória.
- Uma transação de **entrada** só aceita categoria de entrada, e uma de **saída**, categoria de saída.
- Não é possível excluir uma **conta** que tenha transações ou metas, nem uma **categoria** usada em transações. O tipo de uma categoria com transações também não pode ser alterado. As chaves estrangeiras usam `Restrict` para garantir o mesmo no banco.
- Em listagens e no resumo, a data final não pode ser anterior à inicial.
- O **resumo** sem datas considera o mês corrente.

### Tratamento de erros

Todos os erros saem no formato `ProblemDetails` (RFC 9457), com a mensagem pronta para o usuário em `detail`:

| Situação | Status |
| --- | --- |
| Registro não encontrado | 404 |
| Dados inválidos ou regra de negócio violada | 400 |
| Conflito (exclusão com vínculos, tipo imutável) | 409 |
| Violação de chave estrangeira ou índice único no SQL Server | 409 |
| Erro inesperado (registrado em log) | 500 |

### Datas e fuso horário

Datas de negócio (data da transação, data da meta) usam `DateOnly`. Carimbos de criação e atualização são `DateTime` em UTC e saem da API com o sufixo `Z`. O relógio é injetado via `TimeProvider`, o que torna os testes determinísticos. No front-end, as datas trafegam como `yyyy-MM-dd` (veja `core/datas.ts`), porque `toISOString()` converte para UTC e adianta o dia à noite no Brasil.

## Como executar

### Pré-requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 22 ou superior, com npm
- SQL Server (por exemplo, o LocalDB que acompanha o Visual Studio)

### 1. Banco de dados

A connection string de desenvolvimento está em `CMWallet.API/appsettings.Development.json` e aponta para uma instância LocalDB chamada `MinhaInstancia`. Ajuste para o seu ambiente, por exemplo `(localdb)\MSSQLLocalDB`.

Para não versionar credenciais, prefira o user-secrets:

```bash
cd CMWallet.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=CMWallet;..."
```

Em desenvolvimento, a API cria o banco e aplica as migrations ao iniciar (`Database:AplicarMigrationsAoIniciar`). Em outros ambientes, essa opção vem desligada; aplique as migrations manualmente:

```bash
dotnet tool restore
dotnet ef database update --project CMWallet.Infrastructure --startup-project CMWallet.API
```

### 2. API

```bash
dotnet run --project CMWallet.API
```

A API sobe em `http://localhost:5161`. Em desenvolvimento, a documentação interativa (Scalar) fica em `/scalar/v1` e o health check em `/health`. O arquivo `CMWallet.API/CMWallet.API.http` traz requisições de exemplo para cada recurso.

### 3. Front-end

```bash
cd frontend
npm install
npm start
```

Acesse `http://localhost:4200`. O `ng serve` encaminha `/api` para a API (veja `frontend/proxy.conf.json`), então não é preciso configurar CORS no desenvolvimento.

### Primeiros passos no app

Cadastre uma **conta** e ao menos uma **categoria** de entrada e uma de saída. Com isso já é possível lançar transações e ver a visão geral do mês.

### Testes

```bash
dotnet test                    # back-end (xUnit)
cd frontend && npm test        # front-end (Vitest)
```

## Endpoints

| Recurso | Rota base | Operações |
| --- | --- | --- |
| Contas | `/api/contas` | `GET` (lista e por id), `POST`, `PUT`, `DELETE` |
| Categorias | `/api/categorias` | `GET` (lista e por id), `POST`, `PUT`, `DELETE` |
| Transações | `/api/transacoes` | `GET` (lista paginada e por id), `POST`, `PUT`, `DELETE` |
| Metas | `/api/metas` | `GET` (lista e por id), `POST`, `PUT`, `DELETE` |
| Resumo | `/api/resumo` | `GET` com `dataInicio`, `dataFim` e `contaId` opcionais |

Exemplo de listagem filtrada: `GET /api/transacoes?contaId=1&dataInicio=2026-10-01&dataFim=2026-10-31&pagina=1&tamanhoPagina=20`. O tamanho máximo de página é 100.

## Estrutura do repositório

```text
CMWallet.API/              Controllers por recurso, Program.cs, middleware
CMWallet.Application/      Dtos, Services, Interfaces, Exceptions
CMWallet.Domain/           Entities, Enums
CMWallet.Infrastructure/   Persistence (DbContext e Configurations), Repositories, Migrations
CMWallet.Tests/            Testes dos serviços
frontend/                  Angular (core, shared, features, styles)
```

## Próximos passos

- Autenticação e vínculo dos dados a cada usuário
- Testes de integração para os repositórios (a consulta de saldo e o resumo só são exercitados com um banco real)
- Pipeline de CI com build e testes
- Dockerfile e `docker-compose` com SQL Server
- Transferências entre contas e lançamentos recorrentes


# 10-tecnologias-e-padroes

## Tecnologias principais

- **Linguagem**: C# 14  
- **Plataforma**: .NET 10  
- **IDE prevista**: Visual Studio 2026 Insiders  
- **Banco de dados**: PostgreSQL remoto no Supabase  
  - O Supabase já provê a instância PostgreSQL.  
  - O projeto cria e evolui o schema via migrations do Entity Framework Core.  
  - A conexão deve usar a connection string do Supabase (Postgres), preferencialmente via variáveis de ambiente/GitHub Secrets em produção/CI e `appsettings.Development.json` apenas para desenvolvimento local.  
- **Acesso a dados**:  
  - Utilizar **Entity Framework Core** para facilitar o mapeamento objeto‑relacional.  
  - Provider do EF Core: **Npgsql (PostgreSQL)**.  
  - Opcionalmente, podem ser usados comandos SQL diretos onde necessário, mas o padrão deve ser EF Core.  
- **API HTTP**:  
  - Projeto ASP.NET Core (target .NET 10).  
  - Pode ser implementada como **Minimal API** ou com **Controllers**; a preferência deve ser definida na arquitetura, mas manter tudo bem organizado.  
- **MCP Server**:  
  - Projeto de console em .NET 10 que implementa o protocolo MCP para exposição de tools ao GitHub Copilot.  
  - O MCP Server será responsável por orquestrar as chamadas aos casos de uso (camada de Aplicação).

## Padrões e princípios

1. **DDD (Domain‑Driven Design) simplificado**
   - Separar claramente:
     - **Domínio**: entidades, agregados, enums, interfaces de repositório.
     - **Aplicação**: casos de uso, DTOs de entrada/saída, serviços de aplicação.
     - **Infraestrutura**: implementação dos repositórios, DbContext, migrações.
     - **Api**: endpoints HTTP de entrada, mapeamento entre DTOs de requisição/resposta e casos de uso.
     - **McpServer**: adaptação das tools MCP para casos de uso da Aplicação.
   - O domínio deve ter o mínimo de dependências externas possível.

2. **Separação de responsabilidades**
   - A camada de **Api** não deve conter lógica de negócio; apenas orquestração e mapeamento de entrada/saída.
   - A camada de **McpServer** também não deve conter lógica de negócio; apenas traduzir chamadas MCP em chamadas para a camada de Aplicação.
   - Toda regra de negócio deve estar na camada de **Aplicação** (casos de uso) e, quando fizer sentido, na camada de **Domínio** (métodos de entidades, invariantes).

3. **Injeção de dependência**
   - Usar o container padrão do .NET (DI configurado a partir de `Host.CreateDefaultBuilder` ou equivalente no .NET 10).
   - Registrar serviços de aplicação, repositórios e DbContext na inicialização.
   - Tanto a **Api** quanto o **McpServer** devem reutilizar a mesma configuração de DI, idealmente através de um método de extensão compartilhado (por exemplo, em `CatalogoDeMidia.Infraestrutura` ou `CatalogoDeMidia.Aplicacao`).

4. **Persistência / PostgreSQL (Supabase)**
   - O banco PostgreSQL será remoto no Supabase, portanto **não existe arquivo `.db` local** nem pasta `Data` para armazenamento de banco.
   - A aplicação deve usar Entity Framework Core com provider **Npgsql** (`UseNpgsql`) apontando para a **connection string do Supabase/Postgres**.
   - Deve existir uma migração inicial que cria a tabela principal de mídias **no Postgres do Supabase**, e as migrations seguintes devem evoluir o schema no banco remoto.
   - A Infraestrutura deve registrar o `CatalogoDeMidiaDbContext` via `AddDbContext` com `UseNpgsql`, usando a connection string lida de configuração/Secrets.

   **Mini‑bloco de instruções de banco (obrigatório):**
   - **Provider/DI:** o `CatalogoDeMidiaDbContext` DEVE ser registrado via `AddDbContext` usando **`UseNpgsql(connectionString)`**. Não usar `UseSqlite`.
   - **Connection string:** deve vir do Supabase (Postgres). Para dev local pode ficar em `appsettings.Development.json`; para CI/produção deve vir de variáveis de ambiente/GitHub Secrets.
   - **Datas auditáveis:** quando mapear `DataCriacao` e `DataAtualizacao`, usar default SQL compatível com Postgres, por exemplo  
     `HasDefaultValueSql("NOW()")` ou `CURRENT_TIMESTAMP`, conforme o padrão de mapeamento do projeto.
   - **Tipos recomendados:** `Guid` → `uuid`, `DateTimeOffset` → `timestamptz`, `decimal` → `numeric(precision, scale)` (definir precisão/escala nas configurações de entidade).
   - **Naming:** seguir a convenção definida nas instructions (ex.: snake_case) para tabelas/colunas no Postgres.

5. **Tratamento de erros e validações**
   - Casos de uso devem realizar validações de regras de negócio (por exemplo: título obrigatório, ano razoável, tipo de mídia válido, nota dentro de um intervalo definido).
   - Erros de validação devem ser retornados de forma clara para:
     - API (HTTP status adequado, como 400 Bad Request);
     - MCP (mensagem de erro amigável para o usuário da IA).
   - Exceções técnicas (falha de conexão, erro no banco) devem ser logadas e propagadas de forma controlada.

6. **Logging**
   - Tanto a Api quanto o McpServer devem utilizar logging via `ILogger<T>`.
   - Eventos importantes: criação de mídia, avaliação atualizada, consultas com filtros, erros ao acessar o banco.

7. **Comentários e documentação**
   - Classes e métodos públicos devem ter comentários XML (`///`) com descrições claras em português do Brasil.
   - Os arquivos `.md` desta pasta `instructions` serão usados pelo GitHub Copilot para entender o contexto e gerar código alinhado com essas regras.

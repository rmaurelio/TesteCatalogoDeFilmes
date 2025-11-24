# 20-arquitetura-ddd-e-dependencias

## Visão geral da arquitetura em camadas

A solução **CatalogoDeMidia.sln** será composta por cinco projetos principais:

1. **CatalogoDeMidia.Dominio**
2. **CatalogoDeMidia.Aplicacao**
3. **CatalogoDeMidia.Infraestrutura**
4. **CatalogoDeMidia.Api**
5. **CatalogoDeMidia.McpServer**

### 1. CatalogoDeMidia.Dominio

Responsabilidades:

- Contém as entidades de domínio e regras essenciais.
- Contém enums e tipos de valor.
- Define interfaces de repositório, sem conhecimento de como o banco é implementado.

Elementos principais:

- Entidade `Midia`
  - Propriedades:
    - `Id` (ex.: `Guid` ou `int`, definir de forma consistente em toda a solução; preferir `Guid` para evitar conflitos);
    - `Titulo` (string, obrigatório);
    - `AnoLancamento` (int);
    - `Tipo` (enum `TipoMidia`: `Filme`, `Serie`);
    - `Genero` (string ou enum `GeneroMidia` – a definir);
    - `Nota` (decimal? double? – definir tipo numérico para nota; sugerido `decimal` com precisão adequada);
    - `Assistido` (bool);
    - `DataCriacao` (DateTime ou DateTimeOffset);
    - `DataAtualizacao` (DateTime ou DateTimeOffset).
  - Métodos de domínio possíveis:
    - `DefinirNota(decimal novaNota)`
    - `MarcarComoAssistido()`
    - `AtualizarDadosBasicos(...)` (para atualizações gerais).

- Enum `TipoMidia`
  - Valores: `Filme`, `Serie`.

- (Opcional) Enum `GeneroMidia`
  - Valores: `Terror`, `Comedia`, `Drama`, `Acao`, etc.

- Interface de repositório `IMidiaRepositorio`
  - Métodos principais (assinaturas detalhadas na camada de Aplicacao):
    - `AdicionarAsync(Midia midia)`
    - `AtualizarAsync(Midia midia)`
    - `ObterPorIdAsync(Guid id)`
    - `ObterPorTituloAsync(string titulo)` (podendo retornar múltiplos resultados ou lista)
    - `ListarAsync(...)` com parâmetros de filtro.

Dependências:

- **Não deve depender de nenhum outro projeto da solução.**
- Pode depender apenas de pacotes básicos do .NET (por exemplo, tipos primitivos e `System`).

### 2. CatalogoDeMidia.Aplicacao

Responsabilidades:

- Implementar **casos de uso** (aplicação).
- Coordenar as operações entre o domínio e o repositório.
- Expor interfaces de serviços de aplicação para camadas superiores (Api, McpServer).
- Definir DTOs de entrada e saída dos casos de uso.

Casos de uso previstos:

1. `AdicionarMidiaUseCase`
   - Interface: `IAdicionarMidiaUseCase`
   - Método principal:
     - `Task<MidiaRespostaDto> ExecutarAsync(AdicionarMidiaRequisicaoDto requisicao)`
2. `ListarMidiasUseCase`
   - Interface: `IListarMidiasUseCase`
   - Método principal:
     - `Task<IReadOnlyList<MidiaRespostaDto>> ExecutarAsync(ListarMidiasRequisicaoDto filtros)`
3. `AvaliarMidiaUseCase`
   - Interface: `IAvaliarMidiaUseCase`
   - Método principal:
     - `Task<MidiaRespostaDto> ExecutarAsync(AvaliarMidiaRequisicaoDto requisicao)`

DTOs principais:

- `AdicionarMidiaRequisicaoDto`
  - `string Titulo`
  - `int AnoLancamento`
  - `TipoMidia Tipo`
  - `string? Genero`
  - `decimal? Nota`
  - `bool? Assistido`

- `ListarMidiasRequisicaoDto`
  - `TipoMidia? Tipo`
  - `bool? Assistido`
  - `string? Genero`
  - `decimal? NotaMinima`
  - `decimal? NotaMaxima`

- `AvaliarMidiaRequisicaoDto`
  - `Guid? IdMidia`
  - `string? Titulo`
  - `decimal NovaNota`

- `MidiaRespostaDto`
  - `Guid Id`
  - `string Titulo`
  - `int AnoLancamento`
  - `TipoMidia Tipo`
  - `string? Genero`
  - `decimal? Nota`
  - `bool Assistido`
  - `DateTimeOffset DataCriacao`
  - `DateTimeOffset DataAtualizacao`

Dependências:

- Depende de **CatalogoDeMidia.Dominio**.
- Não deve depender de **Infraestrutura**, **Api** nem **McpServer**.

### 3. CatalogoDeMidia.Infraestrutura

Responsabilidades:

- Implementar as interfaces de repositório definidas no domínio.
- Configurar o **DbContext** do Entity Framework Core para PostgreSQL remoto no (Supabase).
- Configurar migrações e aplicação do schema no banco PostgreSQL remoto no (Supabase).
- Fornecer métodos de extensão de DI para registrar repositórios e contexto.

Elementos principais:

- `CatalogoDeMidiaDbContext` (derivado de `DbContext`).
  - `DbSet<Midia> Midias { get; set; }`
  - Configuração da tabela `Midias` (nome pode ser `Midias` ou `Midia`, definir consistentemente).
- Implementação de `IMidiaRepositorio`:
  - Classe `MidiaRepositorio` que usa `CatalogoDeMidiaDbContext`.

- Classe de extensão para registrar serviços:
  - Exemplo: `ServicoDeInfraestruturaConfiguracao`
  - Método estático:
    - `IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, string connectionString)`

Dependências:

- Depende de:
  - **CatalogoDeMidia.Dominio**
- Será referenciado por:
  - **CatalogoDeMidia.Api**
  - **CatalogoDeMidia.McpServer**

### 4. CatalogoDeMidia.Api

Responsabilidades:

- Expor uma API HTTP para consumo via navegador, Swagger ou qualquer outro cliente.
- Traduzir requisições HTTP em chamadas aos casos de uso da Aplicacao.
- Retornar respostas em formato JSON (usando DTOs de resposta).

Endpoints esperados:

- `POST /midias`
  - Chama `IAdicionarMidiaUseCase`.
- `GET /midias`
  - Chama `IListarMidiasUseCase` com filtros via query string.
- `POST /midias/{id}/avaliar`
  - Chama `IAvaliarMidiaUseCase` identificando a mídia por `id`.

Dependências:

- Depende de:
  - **CatalogoDeMidia.Aplicacao**
  - **CatalogoDeMidia.Dominio**
  - **CatalogoDeMidia.Infraestrutura**

### 5. CatalogoDeMidia.McpServer

Responsabilidades:

- Implementar um servidor MCP em .NET 10.
- Registrar as tools MCP:
  - `adicionar_midia`
  - `listar_midias`
  - `avaliar_midia`
- Orquestrar chamadas do MCP para os casos de uso da camada de Aplicacao.
- Utilizar a mesma configuração de DI e acesso a banco que a Api.

Dependências:

- Depende de:
  - **CatalogoDeMidia.Aplicacao**
  - **CatalogoDeMidia.Dominio**
  - **CatalogoDeMidia.Infraestrutura**

Regras de dependência (resumo):

- Dominio → não depende de ninguém.
- Aplicacao → depende apenas de Dominio.
- Infraestrutura → depende de Dominio.
- Api → depende de Aplicacao, Dominio, Infraestrutura.
- McpServer → depende de Aplicacao, Dominio, Infraestrutura.


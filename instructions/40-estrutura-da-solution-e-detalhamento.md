# 40-estrutura-da-solution-e-detalhamento

## Estrutura da solution

A solution deve conter os seguintes projetos:

- `CatalogoDeMidia.Dominio`
- `CatalogoDeMidia.Aplicacao`
- `CatalogoDeMidia.Infraestrutura`
- `CatalogoDeMidia.Api`
- `CatalogoDeMidia.McpServer`

Cada projeto deve ser criado como **.NET 10**, usando **C# 14**.

### Referências entre projetos

- `CatalogoDeMidia.Dominio`
  - Não referencia nenhum outro projeto.

- `CatalogoDeMidia.Aplicacao`
  - Referencia:
    - `CatalogoDeMidia.Dominio`

- `CatalogoDeMidia.Infraestrutura`
  - Referencia:
    - `CatalogoDeMidia.Dominio`

- `CatalogoDeMidia.Api`
  - Referencia:
    - `CatalogoDeMidia.Dominio`
    - `CatalogoDeMidia.Aplicacao`
    - `CatalogoDeMidia.Infraestrutura`

- `CatalogoDeMidia.McpServer`
  - Referencia:
    - `CatalogoDeMidia.Dominio`
    - `CatalogoDeMidia.Aplicacao`
    - `CatalogoDeMidia.Infraestrutura`

## Estrutura interna de cada projeto

### Projeto CatalogoDeMidia.Dominio

Pastas sugeridas:

- `Entidades`
- `Enums`
- `Repositorios`

Classes e interfaces principais:

1. `Entidades/Midia.cs`
   - Propriedades:
     - `Guid Id`
     - `string Titulo`
     - `int AnoLancamento`
     - `TipoMidia Tipo`
     - `string? Genero`
     - `decimal? Nota`
     - `bool Assistido`
     - `DateTimeOffset DataCriacao`
     - `DateTimeOffset DataAtualizacao`
   - Métodos:
     - `void DefinirNota(decimal novaNota)`
     - `void MarcarComoAssistido()`
     - `void AtualizarDadosBasicos(string novoTitulo, int novoAno, TipoMidia novoTipo, string? novoGenero)`

2. `Enums/TipoMidia.cs`
   - Enum `TipoMidia` com valores:
     - `Filme`
     - `Serie`

3. (Opcional) `Enums/GeneroMidia.cs`
   - Enum `GeneroMidia` com valores comuns (Terror, Comedia, etc.), caso seja utilizado.

4. `Repositorios/IMidiaRepositorio.cs`
   - Métodos principais:
     - `Task AdicionarAsync(Midia midia, CancellationToken cancellationToken = default)`
     - `Task AtualizarAsync(Midia midia, CancellationToken cancellationToken = default)`
     - `Task<Midia?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)`
     - `Task<IReadOnlyList<Midia>> ObterPorTituloAsync(string titulo, CancellationToken cancellationToken = default)`
     - `Task<IReadOnlyList<Midia>> ListarAsync(...)` com parâmetros para filtros (definidos na Aplicacao).

### Projeto CatalogoDeMidia.Aplicacao

Pastas sugeridas:

- `CasosDeUso`
  - `AdicionarMidia`
  - `ListarMidias`
  - `AvaliarMidia`
- `Dtos`
  - `Requisicoes`
  - `Respostas`
- `Interfaces`

Interfaces e classes principais:

1. `Interfaces/IAdicionarMidiaUseCase.cs`
   - `Task<MidiaRespostaDto> ExecutarAsync(AdicionarMidiaRequisicaoDto requisicao, CancellationToken cancellationToken = default)`

2. `Interfaces/IListarMidiasUseCase.cs`
   - `Task<IReadOnlyList<MidiaRespostaDto>> ExecutarAsync(ListarMidiasRequisicaoDto filtros, CancellationToken cancellationToken = default)`

3. `Interfaces/IAvaliarMidiaUseCase.cs`
   - `Task<MidiaRespostaDto> ExecutarAsync(AvaliarMidiaRequisicaoDto requisicao, CancellationToken cancellationToken = default)`

4. `Dtos/Requisicoes/AdicionarMidiaRequisicaoDto.cs`
5. `Dtos/Requisicoes/ListarMidiasRequisicaoDto.cs`
6. `Dtos/Requisicoes/AvaliarMidiaRequisicaoDto.cs`
7. `Dtos/Respostas/MidiaRespostaDto.cs`

8. `CasosDeUso/AdicionarMidia/AdicionarMidiaUseCase.cs`
   - Depende de `IMidiaRepositorio`.
   - Converte DTO de requisição → entidade `Midia`.
   - Aplica regras de negócio (ex.: validar campos, definir datas).
   - Persiste a mídia via repositório.
   - Converte entidade → `MidiaRespostaDto`.

9. `CasosDeUso/ListarMidias/ListarMidiasUseCase.cs`
   - Depende de `IMidiaRepositorio`.
   - Constrói filtros de consulta.
   - Chama método `ListarAsync(...)` do repositório.
   - Converte lista de entidades → lista de `MidiaRespostaDto`.

10. `CasosDeUso/AvaliarMidia/AvaliarMidiaUseCase.cs`
    - Depende de `IMidiaRepositorio`.
    - Localiza a mídia:
      - Por `Id`, se fornecido;
      - Ou por `Titulo`, se apenas título for fornecido (lidar com múltiplos resultados).
    - Chama método de domínio `DefinirNota`.
    - Atualiza registro no repositório.
    - Retorna `MidiaRespostaDto`.

### Projeto CatalogoDeMidia.Infraestrutura

Pastas sugeridas:

- `Persistencia`
  - `Contexto`
  - `Repositorios`
  - `Configuracoes`

Classes principais:

1. `Persistencia/Contexto/CatalogoDeMidiaDbContext.cs`
   - Deriva de `DbContext`.
   - Propriedade:
     - `DbSet<Midia> Midias { get; set; }`
   - Configuração:
     - Mapear entidade `Midia` para tabela (ex.: `Midias`).
     - Definir chaves, tipos de colunas, restrições.

2. `Persistencia/Repositorios/MidiaRepositorio.cs`
   - Implementa `IMidiaRepositorio`.
   - Usa `CatalogoDeMidiaDbContext` para acessar o banco.

3. Classe de extensão para DI, ex.: `ConfiguracaoInfraestruturaExtensoes.cs`
   - Método:
     - `public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, string connectionString)`
   - Deve:
     - Registrar `CatalogoDeMidiaDbContext` para uso com PostgreSQL (Supabase) via Npgsql;
     - Registrar `IMidiaRepositorio` como implementação de `MidiaRepositorio`.

### Projeto CatalogoDeMidia.Api

Pastas sugeridas:

- `Controllers` ou `Endpoints` (caso Minimal API)
- `Dtos`
  - Reutilizar os DTOs da Aplicacao quando possível, ou criar DTOs específicos para requisição/resposta HTTP e mapear.

Elementos principais (caso MVC/API tradicional):

1. `Controllers/MidiasController.cs`
   - Endpoints:
     - `POST /midias`
     - `GET /midias`
     - `POST /midias/{id}/avaliar`
   - Injeta:
     - `IAdicionarMidiaUseCase`
     - `IListarMidiasUseCase`
     - `IAvaliarMidiaUseCase`

2. `Program.cs`
   - Configuração:
     - `Host.CreateDefaultBuilder(args)` ou equivalente em .NET 10.
     - Registrar serviços da Aplicacao e Infraestrutura.
     - Configurar conexão PostgreSQL do Supabase (connection string).
     - Habilitar Swagger.
     - Mapear controladores / endpoints.

### Projeto CatalogoDeMidia.McpServer

Pastas sugeridas:

- `Ferramentas`
- `Configuracao`

Classes principais:

1. `Program.cs`
   - Configura:
     - Host genérico;
     - Logging;
     - DI (chamando `AdicionarInfraestrutura` e registrando casos de uso da Aplicacao);
     - Inicializa o servidor MCP;
     - Registra as ferramentas (tools) disponíveis.

2. `Ferramentas/FerramentasCatalogoDeMidia.cs`
   - Métodos públicos expostos como tools MCP:
     - `Task<MidiaRespostaDto> AdicionarMidiaAsync(...)`
     - `Task<IReadOnlyList<MidiaRespostaDto>> ListarMidiasAsync(...)`
     - `Task<MidiaRespostaDto> AvaliarMidiaAsync(...)`
   - Internamente cada método MCP chamará o caso de uso correspondente na camada de Aplicacao.

## Considerações finais

- Todos os nomes de classes, métodos, propriedades, comentários e mensagens devem ser em português do Brasil.
- A solução deve ser estruturada de forma que o GitHub Copilot, ao ler estes arquivos `.md`, consiga:
  - Criar os projetos com os nomes corretos;
  - Configurar os targets para .NET 10 e C# 14;
  - Gerar o esqueleto das classes e métodos conforme descrito;
  - Implementar os casos de uso, repositórios, controllers/endpoints e MCP server de forma consistente com esta arquitetura.


# 30-mcp-server-e-tools

## Objetivo do projeto CatalogoDeMidia.McpServer

O projeto **CatalogoDeMidia.McpServer** é responsável por:

- Implementar um **servidor MCP** em .NET 10;
- Disponibilizar tools para o GitHub Copilot (e/ou outros clientes MCP compatíveis);
- Traduzir solicitações em linguagem natural, vindas do chat de IA, em chamadas aos casos de uso da solução;
- Utilizar a mesma base de dados PostgreSQL remoto no (Supabase) que a API.

Este projeto é **obrigatório** e é o ponto central para testes com IA + MCP.

## Tools MCP previstas

As tools MCP devem ter nomes em português do Brasil, coerentes com a linguagem da solução.

Tools obrigatórias:

1. `adicionar_midia`
2. `listar_midias`
3. `avaliar_midia`

### 1. Tool `adicionar_midia`

Responsabilidade:

- Criar uma nova mídia (filme ou série) no catálogo.

Parâmetros de entrada (exemplo de modelo conceitual):

- `titulo` (string, obrigatório)
- `ano_lancamento` (inteiro, obrigatório)
- `tipo` (string, obrigatório; valores possíveis: `"Filme"` ou `"Serie"`)
- `genero` (string, opcional)
- `nota` (numérico, opcional)
- `assistido` (booleano, opcional; padrão `false`)

Fluxo:

1. Validar parâmetros de entrada (camada de Aplicacao fará validações mais profundas).
2. Montar um `AdicionarMidiaRequisicaoDto`.
3. Chamar `IAdicionarMidiaUseCase.ExecutarAsync(requisicao)`.
4. Receber um `MidiaRespostaDto`.
5. Retornar os dados da mídia criada como saída da tool.

Saída:

- Objeto que representa a mídia criada, com os campos:
  - `id`
  - `titulo`
  - `ano_lancamento`
  - `tipo`
  - `genero`
  - `nota`
  - `assistido`
  - `data_criacao`
  - `data_atualizacao`

### 2. Tool `listar_midias`

Responsabilidade:

- Listar mídias cadastradas no catálogo, com possibilidade de filtros.

Parâmetros de entrada (todos opcionais):

- `tipo` (string; `"Filme"` ou `"Serie"`)
- `assistido` (booleano)
- `genero` (string)
- `nota_minima` (numérico)
- `nota_maxima` (numérico)

Fluxo:

1. Montar um `ListarMidiasRequisicaoDto` com base nos parâmetros recebidos.
2. Chamar `IListarMidiasUseCase.ExecutarAsync(filtros)`.
3. Receber uma lista de `MidiaRespostaDto`.
4. Retornar a lista de mídias como saída da tool.

Saída:

- Lista de objetos representando mídias, cada um com os campos:
  - `id`, `titulo`, `ano_lancamento`, `tipo`, `genero`, `nota`, `assistido`, `data_criacao`, `data_atualizacao`.

### 3. Tool `avaliar_midia`

Responsabilidade:

- Definir ou atualizar a nota de uma mídia existente.

Parâmetros de entrada:

- `id_midia` (Guid, opcional)
- `titulo` (string, opcional)
- `nova_nota` (numérico, obrigatório)

Regras:

- Pelo menos um identificador deve ser fornecido:
  - `id_midia` OU `titulo`.
- Se apenas `titulo` for informado e houver mais de uma mídia com o mesmo título, o caso de uso deve:
  - Retornar erro com mensagem clara;
  - Ou definir uma estratégia (por exemplo, escolher a mais recente), conforme especificação na camada de Aplicacao.

Fluxo:

1. Montar um `AvaliarMidiaRequisicaoDto` com os dados recebidos.
2. Chamar `IAvaliarMidiaUseCase.ExecutarAsync(requisicao)`.
3. Receber um `MidiaRespostaDto`.
4. Retornar a mídia atualizada como saída da tool.

Saída:

- Objeto com o estado atualizado da mídia, incluindo a nova nota.

## Estrutura geral do projeto McpServer

Arquivos e classes esperados (nomes sugestivos):

- `Program.cs`
  - Configura:
    - Host genérico (`Host.CreateDefaultBuilder` ou equivalente em .NET 10);
    - Logging;
    - Injeção de dependência (chamando métodos de extensão da Infraestrutura e Aplicacao);
    - Inicialização do servidor MCP;
    - Registro das tools `adicionar_midia`, `listar_midias`, `avaliar_midia`.

- Pasta `Ferramentas` ou `Tools` (nomes em português, ex.: `FerramentasCatalogoMidia.cs`)
  - Classe (ou classes) responsáveis por expor os métodos MCP:
    - `FerramentasCatalogoMidia`
      - Método MCP `adicionar_midia` → chama `IAdicionarMidiaUseCase`;
      - Método MCP `listar_midias` → chama `IListarMidiasUseCase`;
      - Método MCP `avaliar_midia` → chama `IAvaliarMidiaUseCase`.

- Integração com a DI:
  - Os casos de uso e repositórios devem ser resolvidos via construtor (injeção de dependência).

## Comportamento esperado no uso via GitHub Copilot

- Quando o usuário digitar algo como:
  - “cadastre o filme Matrix, de 1999, como filme de ação com nota 10”,
- O GitHub Copilot deve:
  - Entender a intenção;
  - Selecionar a tool `adicionar_midia`;
  - Preencher os argumentos (`titulo`, `ano_lancamento`, `tipo`, `genero`, `nota`);
  - Enviar a chamada ao MCP Server;
  - Receber o resultado da criação;
  - Apresentar ao usuário um resumo com os dados da mídia cadastrada.

Da mesma forma:

- “quais são os filmes de terror?” → tool `listar_midias` com `tipo = Filme` e `genero = Terror`.
- “atualize a nota do filme Alien para 9,5” → tool `avaliar_midia` com `titulo = Alien` e `nova_nota = 9.5`.

Toda a lógica de consulta e gravação efetiva fica nos casos de uso e repositórios, e o MCP Server apenas encaminha as chamadas.


# 00-visao-geral-e-objetivos

## Nome da solução

A solução se chamará **CatalogoDeMidia.sln**.

## Contexto e objetivo do projeto

Este projeto tem como objetivo criar um **catálogo pessoal de mídias** (filmes e séries), permitindo:

- cadastrar mídias (filmes e séries);
- listar mídias com filtros (tipo, se foi assistida, etc.);
- avaliar mídias (atribuir nota).

O foco principal NÃO é apenas o CRUD em si, mas:

1. **Aprender e praticar desenvolvimento com IA e MCP**:
   - Criar um **MCP Server** em .NET 10 e C# 14;
   - Registrar esse MCP Server no GitHub Copilot;
   - Permitir que o usuário converse, em linguagem natural, no chat do GitHub Copilot, e o Copilot utilize as **tools MCP** para:
     - cadastrar mídias;
     - listar mídias;
     - avaliar mídias.

2. **Aplicar uma arquitetura em camadas com DDD**:
   - Separar responsabilidades em projetos distintos:
     - Dominio (regra de negócio);
     - Aplicacao (casos de uso);
     - Infraestrutura (acesso a dados via EF Core com PostgreSQL remoto no Supabase);
     - Api (exposição HTTP para consumo tradicional);
     - McpServer (exposição das tools MCP para o Copilot).

3. **Persistência remota usando PostgreSQL no Supabase**:
   - Banco de dados PostgreSQL remoto provisionado no Supabase; não existe arquivo local de banco.
   - Modelo relacional simples, mas bem definido, para mídia (filmes/séries), criado e evoluído via migrations do EF Core aplicadas no Postgres do Supabase.
   - A Infraestrutura deve acessar o banco via `CatalogoDeMidiaDbContext` configurado com provider Npgsql (`UseNpgsql`) e connection string do Supabase.

## Funcionalidades principais

Funcionalidades a serem expostas tanto via API quanto via MCP (de forma consistente):

1. **Adicionar mídia**
   - Permite cadastrar uma nova mídia (filme ou série).
   - Campos: título, ano, tipo, gênero (opcional mas recomendado), nota inicial (opcional), se já foi assistido (opcional, padrão falso).

2. **Listar mídias**
   - Permite consultar a lista de mídias cadastradas.
   - Permite filtros opcionais:
     - tipo (filme ou série);
     - se foi assistido (verdadeiro/falso);
     - gênero (terror, comédia, drama, etc.);
     - faixa de nota (ex.: nota mínima).

3. **Avaliar mídia**
   - Permite registrar ou atualizar a nota de uma mídia já cadastrada.
   - A identificação pode ser feita por **Id** (preferencialmente) ou, em alguns casos, por título (podendo haver regra para lidar com duplicidades).

## Interações esperadas com IA (via MCP)

Exemplos de interações em linguagem natural que o projeto deve suportar via MCP:

- “Cadastre o filme ‘Alien, o oitavo passageiro’, de 1979, como filme de terror, com nota 9.”
- “Liste todas as séries que ainda não foram assistidas.”
- “Quais são os filmes de terror com nota maior ou igual a 8?”
- “Atualize a nota do filme ‘Matrix’ para 10.”
- “Mostre todas as mídias cadastradas, ordenadas por nota.”

O **MCP Server** deverá expor tools com nomes e contratos bem definidos, de forma que o GitHub Copilot consiga:

- interpretar a intenção do usuário;
- mapear para a tool correta;
- enviar os parâmetros corretos;
- receber os dados da aplicação;
- devolver uma resposta em linguagem natural ao usuário final.

## Regras gerais de nomenclatura

- Tudo deve ser escrito em **português do Brasil**:
  - nomes de projetos;
  - nomes de classes;
  - nomes de interfaces;
  - nomes de métodos;
  - nomes de arquivos (quando fizer sentido);
  - nomes das tools MCP.
- Interfaces devem seguir o padrão de prefixo `I` (ex.: `IMidiaRepositorio`, `IAdicionarMidiaUseCase`).
- Entidades de domínio devem usar nomes substantivos no singular (ex.: `Midia`).
- Casos de uso devem usar o padrão `<Verbo><Entidade>UseCase` (ex.: `AdicionarMidiaUseCase`, `ListarMidiasUseCase`, `AvaliarMidiaUseCase`).

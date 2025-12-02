# CatalogoDeMidia.Infraestrutura

Este projeto contém a implementação de acesso a dados, DbContext do Entity Framework Core e implementações de repositórios.

## Estrutura de Pastas

- **Persistencia**: Contém toda a lógica de persistência de dados
  - **Contexto**: Contém o DbContext (CatalogoDeMidiaDbContext)
  - **Repositorios**: Contém as implementações dos repositórios (MidiaRepositorio)
  - **Configuracoes**: Contém as configurações de entidades do EF Core

## Responsabilidades

- Implementar as interfaces de repositório definidas no domínio
- Configurar o DbContext do Entity Framework Core para SQLite
- Configurar migrações e aplicação do schema no banco
- Fornecer métodos de extensão de DI para registrar repositórios e contexto

## Dependências

- **CatalogoDeMidia.Dominio**: Para implementar as interfaces de repositório

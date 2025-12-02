# CatalogoDeMidia.Aplicacao

Este projeto contém os casos de uso, DTOs e interfaces de serviços de aplicação para o sistema de catálogo de mídias.

## Estrutura de Pastas

- **CasosDeUso**: Contém as implementações dos casos de uso
  - **AdicionarMidia**: Caso de uso para adicionar novas mídias
  - **ListarMidias**: Caso de uso para listar mídias com filtros
  - **AvaliarMidia**: Caso de uso para avaliar/atribuir nota a mídias
- **Dtos**: Contém os Data Transfer Objects
  - **Requisicoes**: DTOs de entrada dos casos de uso
  - **Respostas**: DTOs de saída dos casos de uso
- **Interfaces**: Contém as interfaces dos casos de uso

## Responsabilidades

- Implementar casos de uso (lógica de aplicação)
- Coordenar operações entre domínio e repositórios
- Definir DTOs de entrada e saída
- Expor interfaces de serviços para camadas superiores (Api, McpServer)

## Dependências

- **CatalogoDeMidia.Dominio**: Para acesso às entidades e interfaces de repositório

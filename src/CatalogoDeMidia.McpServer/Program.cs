// CatalogoDeMidia.McpServer - Servidor MCP para integração com GitHub Copilot
// Responsável por expor ferramentas (tools) que permitem ao GitHub Copilot
// gerenciar o catálogo de mídias através de linguagem natural

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Configuração do host genérico
var builder = Host.CreateApplicationBuilder(args);

// Configuração de logging
// =======================
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configuração de Injeção de Dependência
// ======================================

// TODO: Registrar serviços da camada de Aplicação (casos de uso)
// Reaproveitar a mesma configuração utilizada na API
// Exemplo:
// builder.Services.AddScoped<IAdicionarMidiaUseCase, AdicionarMidiaUseCase>();
// builder.Services.AddScoped<IListarMidiasUseCase, ListarMidiasUseCase>();
// builder.Services.AddScoped<IAvaliarMidiaUseCase, AvaliarMidiaUseCase>();

// TODO: Registrar serviços da camada de Infraestrutura
// Usar o mesmo método de extensão da API
// Exemplo:
// var connectionString = builder.Configuration.GetConnectionString("CatalogoDeMidia") 
//     ?? "Data Source=catalogodemidia.db";
// builder.Services.AdicionarInfraestrutura(connectionString);

// TODO: Registrar as ferramentas MCP
// Exemplo:
// builder.Services.AddSingleton<FerramentasCatalogoDeMidia>();

// Construção do host
var host = builder.Build();

// TODO: Inicializar o servidor MCP e registrar as ferramentas
// O servidor MCP será responsável por:
// - Escutar conexões de clientes MCP (como o GitHub Copilot)
// - Expor as ferramentas disponíveis:
//   - adicionar_midia: Adiciona uma nova mídia ao catálogo
//   - listar_midias: Lista mídias com filtros opcionais
//   - avaliar_midia: Atribui ou atualiza a nota de uma mídia
// - Traduzir chamadas MCP em chamadas aos casos de uso da camada de Aplicação

// TODO: Implementar a integração real com o protocolo MCP
// Exemplo com a biblioteca MCP para .NET:
// using var mcpServer = host.Services.GetRequiredService<McpServer>();
// await mcpServer.RegistrarFerramenta("adicionar_midia", ...);
// await mcpServer.RegistrarFerramenta("listar_midias", ...);
// await mcpServer.RegistrarFerramenta("avaliar_midia", ...);
// await mcpServer.IniciarAsync();

// Execução do host
await host.RunAsync();

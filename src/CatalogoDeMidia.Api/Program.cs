// CatalogoDeMidia.Api - Ponto de entrada da aplicação Web API
// Responsável por expor endpoints HTTP para gerenciamento do catálogo de mídias

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
// ========================

// Adiciona suporte a controllers
builder.Services.AddControllers();

// Adiciona suporte ao OpenAPI/Swagger para documentação da API
builder.Services.AddOpenApi();

// TODO: Registrar serviços da camada de Aplicação (casos de uso)
// Exemplo:
// builder.Services.AddScoped<IAdicionarMidiaUseCase, AdicionarMidiaUseCase>();
// builder.Services.AddScoped<IListarMidiasUseCase, ListarMidiasUseCase>();
// builder.Services.AddScoped<IAvaliarMidiaUseCase, AvaliarMidiaUseCase>();

// TODO: Registrar serviços da camada de Infraestrutura
// Exemplo:
// builder.Services.AdicionarInfraestrutura(connectionString);

// TODO: Configurar a conexão com o banco de dados SQLite
// Exemplo:
// var connectionString = builder.Configuration.GetConnectionString("CatalogoDeMidia") 
//     ?? "Data Source=catalogodemidia.db";

var app = builder.Build();

// Configuração do pipeline HTTP
// =============================

// Habilita o Swagger/OpenAPI apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redireciona HTTP para HTTPS
app.UseHttpsRedirection();

// Adiciona autorização (preparado para uso futuro)
app.UseAuthorization();

// Mapeia os controllers
app.MapControllers();

// TODO: Mapear endpoints de mídias (se usar Minimal API em vez de Controllers)
// Exemplo com Minimal API:
// app.MapPost("/midias", async (IAdicionarMidiaUseCase useCase, AdicionarMidiaRequisicaoDto requisicao) => ...);
// app.MapGet("/midias", async (IListarMidiasUseCase useCase, [AsParameters] ListarMidiasRequisicaoDto filtros) => ...);
// app.MapPost("/midias/{id}/avaliar", async (IAdicionarMidiaUseCase useCase, Guid id, AvaliarMidiaRequisicaoDto requisicao) => ...);

app.Run();

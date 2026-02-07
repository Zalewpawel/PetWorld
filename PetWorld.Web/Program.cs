using PetWorld.Web.Components;
using PetWorld.Infrastructure.Persistence;
using PetWorld.Infrastructure.Repositories;
using PetWorld.Application.Services;
using PetWorld.Domain.Repositories;
using PetWorld.Infrastructure.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

var openAIApiKey = builder.Configuration["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("OpenAI:ApiKey is missing in configuration.");

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var client = new OpenAI.Chat.ChatClient("gpt-4o", openAIApiKey);
    return client.AsIChatClient();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<AgentService>();
builder.Services.AddScoped<IChatService>(sp =>
{
    var chatRepo = sp.GetRequiredService<IChatMessageRepository>();
    var productRepo = sp.GetRequiredService<IProductRepository>();
    var agentService = sp.GetRequiredService<AgentService>();
    return new ChatService(chatRepo, productRepo, agentService);
});
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await EnsureDatabaseAsync(db, app.Logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task EnsureDatabaseAsync(AppDbContext db, ILogger logger)
{
    const int maxRetries = 10;
    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database not ready (attempt {Attempt}/{MaxRetries}).", attempt, maxRetries);
            if (attempt == maxRetries)
            {
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

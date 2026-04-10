using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var appName = Environment.GetEnvironmentVariable("APP_NAME") ?? "Flui Demo ASP.NET Core";
var appVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Flui Demo — ASP.NET Core 10",
        Version = appVersion,
        Description = "A minimal demo application deployed via Flui."
    });
});

builder.Services.AddSingleton<ItemStore>();

var app = builder.Build();

app.UseSwagger(c => c.RouteTemplate = "api/openapi/{documentName}.json");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/openapi/v1.json", "Flui Demo — ASP.NET Core 10");
    c.RoutePrefix = "docs";
});

var startTime = DateTime.UtcNow;

// ─── Home ──────────────────────────────────────────────────────────────────

app.MapGet("/", () => Results.Content($$"""
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>{{appName}}</title>
  <style>
    * { box-sizing: border-box; margin: 0; padding: 0; }
    html, body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: #0a0a0f; color: #e8e8ed; min-height: 100vh;
    }
    a { color: #4f9eff; text-decoration: none; }
    a:hover { text-decoration: underline; }
    .page { max-width: 800px; margin: 0 auto; padding: 4rem 2rem; }
    .badge {
      display: inline-block; padding: 0.4rem 0.9rem; border-radius: 999px;
      background: linear-gradient(135deg, #4f9eff, #a855f7); color: #fff;
      font-size: 0.8rem; font-weight: 600; margin-bottom: 1.5rem;
    }
    h1 { font-size: 2.5rem; margin-bottom: 0.5rem; }
    .subtitle { color: #888; margin-bottom: 2rem; }
    .card {
      background: #15151c; border: 1px solid #2a2a35; border-radius: 12px;
      padding: 1.5rem; margin-bottom: 2rem;
    }
    .card h2 { font-size: 1.2rem; margin-bottom: 1rem; }
    ul { list-style: none; display: grid; gap: 0.5rem; }
    code {
      display: inline-block; background: #2a2a35; color: #4f9eff;
      padding: 0.1rem 0.4rem; border-radius: 4px; font-size: 0.75rem;
      font-weight: 600; margin-right: 0.4rem;
    }
    footer {
      margin-top: 3rem; padding-top: 1.5rem; border-top: 1px solid #2a2a35;
      color: #666; font-size: 0.85rem; text-align: center;
    }
  </style>
</head>
<body>
  <main class="page">
    <div class="badge">🚀 Flui Demo Application</div>
    <h1>{{appName}}</h1>
    <p class="subtitle">ASP.NET Core 10 · Minimal API · Swashbuckle · v{{appVersion}}</p>
    <section class="card">
      <h2>API Endpoints</h2>
      <ul>
        <li><code>GET</code> <a href="/health">/health</a> — health</li>
        <li><code>GET</code> <a href="/items">/items</a> — list items</li>
        <li><code>POST</code> /items — create item</li>
        <li><code>GET</code> <a href="/api/openapi/v1.json">/api/openapi/v1.json</a> — spec</li>
        <li><code>GET</code> <a href="/docs">/docs</a> — Swagger UI</li>
      </ul>
    </section>
    <footer>Powered by <a href="https://flui.cloud">Flui</a></footer>
  </main>
</body>
</html>
""", "text/html"))
    .ExcludeFromDescription();

// ─── Health ────────────────────────────────────────────────────────────────

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    appName,
    version = appVersion,
    uptime = (long)(DateTime.UtcNow - startTime).TotalSeconds,
    timestamp = DateTime.UtcNow.ToString("o")
}))
    .WithTags("Health")
    .WithSummary("Health check");

// ─── Items ─────────────────────────────────────────────────────────────────

app.MapGet("/items", (ItemStore store) =>
        Results.Ok(new { items = store.List() }))
    .WithTags("Items")
    .WithSummary("List items");

app.MapPost("/items", (CreateItemRequest request, ItemStore store) =>
{
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        return Results.BadRequest(new { errors = validationResults.Select(r => r.ErrorMessage) });
    }
    var item = store.Create(request.Name, request.Description);
    return Results.Created($"/items/{item.Id}", item);
})
    .WithTags("Items")
    .WithSummary("Create item");

app.MapGet("/items/{id}", (string id, ItemStore store) =>
{
    var item = store.Get(id);
    return item is null ? Results.NotFound(new { error = "Item not found" }) : Results.Ok(item);
})
    .WithTags("Items")
    .WithSummary("Get item by ID");

app.MapDelete("/items/{id}", (string id, ItemStore store) =>
{
    return store.Delete(id)
        ? Results.Ok(new { deleted = true })
        : Results.NotFound(new { error = "Item not found" });
})
    .WithTags("Items")
    .WithSummary("Delete item by ID");

app.Run();

// ─── Models ────────────────────────────────────────────────────────────────

public record Item(string Id, string Name, string Description, string CreatedAt);

public class CreateItemRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;
}

public class ItemStore
{
    private readonly Dictionary<string, Item> _items = new();

    public ItemStore()
    {
        var now = DateTime.UtcNow.ToString("o");
        _items["1"] = new Item("1", "Welcome to Flui",
            "Your first demo item — feel free to delete it.", now);
        _items["2"] = new Item("2", "Try the API",
            "Visit /docs to explore the OpenAPI documentation.", now);
    }

    public List<Item> List() => _items.Values
        .OrderByDescending(i => i.CreatedAt)
        .ToList();

    public Item? Get(string id) => _items.TryGetValue(id, out var item) ? item : null;

    public Item Create(string name, string description)
    {
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var item = new Item(id, name, description, DateTime.UtcNow.ToString("o"));
        _items[id] = item;
        return item;
    }

    public bool Delete(string id) => _items.Remove(id);
}

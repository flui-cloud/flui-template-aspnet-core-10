# Flui Template — ASP.NET Core 10

A minimal demo application built with **ASP.NET Core 10 LTS** and ready to deploy on [Flui](https://flui.cloud).

This template includes:

- 🟣 ASP.NET Core 10 (.NET 10 LTS) with Minimal API
- 🩺 `/health` endpoint
- 📦 In-memory item store with full CRUD (`/items`)
- 📖 OpenAPI 3 spec via Swashbuckle (`/api/openapi/v1.json`)
- 📚 Swagger UI at `/docs`
- 🐳 Multi-stage Dockerfile (`#flui-managed`)
- ✅ Data annotations validation

## Local development

```bash
dotnet run
```

App runs on http://localhost:5000 (default Kestrel port)

## Build with Docker

```bash
docker build -t flui-demo-aspnet .
docker run -p 8080:8080 flui-demo-aspnet
```

## Environment variables

| Variable | Default | Description |
|----------|---------|-------------|
| `APP_NAME` | `Flui Demo ASP.NET Core` | App name |
| `APP_VERSION` | `1.0.0` | App version |
| `ASPNETCORE_URLS` | `http://+:8080` | Bind URL |

## Deploy with Flui

1. Click **Use this template** on GitHub
2. Connect to Flui
3. Click **Deploy**

## License

MIT

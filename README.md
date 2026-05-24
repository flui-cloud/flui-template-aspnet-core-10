# Flui Template — ASP.NET Core 10

A minimal demo application built with **ASP.NET Core 10 LTS** and ready to deploy on [Flui](https://flui.cloud).

Includes:

- ASP.NET Core 10 LTS Minimal API on .NET 10
- `/health` endpoint
- In-memory item store with full CRUD on `/items`
- OpenAPI 3 spec via Swashbuckle at `/api/openapi/v1.json`
- Swagger UI at `/docs`
- Multi-stage `#flui-managed` Dockerfile
- Data annotations validation

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

This repo ships with a [`flui.yaml`](./flui.yaml) manifest describing the build strategy, port, healthcheck and resource profile.

From the CLI, with `flui` installed and authenticated against your cluster:

```bash
flui deploy ./flui.yaml
```

The CLI reads the manifest, triggers a build via GitHub Actions and rolls out the workload.

From the UI:

1. Click **Use this template** on GitHub.
2. Connect the new repository to Flui.
3. Click **Deploy**.

Built for [Flui](https://github.com/flui-cloud/flui-core) — see the main repo for cluster setup and CLI installation.

## License

MIT

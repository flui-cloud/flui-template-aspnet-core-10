# #flui-managed
# syntax=docker/dockerfile:1.6

# ─── Stage 1: builder ──────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /src

COPY FluiDemo.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ─── Stage 2: runner ───────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runner
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

RUN groupadd --system --gid 1001 dotnet \
 && useradd --system --uid 1001 --gid dotnet --no-create-home aspnet

COPY --from=builder --chown=aspnet:dotnet /app/publish ./

USER aspnet

EXPOSE 8080

ENTRYPOINT ["dotnet", "FluiDemo.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HealthMonitor.Api/HealthMonitor.Api.csproj src/HealthMonitor.Api/
RUN dotnet restore src/HealthMonitor.Api/HealthMonitor.Api.csproj

COPY . .
RUN dotnet publish src/HealthMonitor.Api/HealthMonitor.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "HealthMonitor.Api.dll"]

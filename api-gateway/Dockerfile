# Dockerfile para API Gateway
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Crear directorio para logos con permisos correctos
RUN mkdir -p /app/logos && chmod 777 /app/logos

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["api-gateway.csproj", "./"]
RUN dotnet restore "api-gateway.csproj"

COPY . .
RUN dotnet build "api-gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api-gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Asegurarse de que el directorio de logos existe y tiene permisos
RUN mkdir -p /app/logos && chmod 777 /app/logos

ENTRYPOINT ["dotnet", "api-gateway.dll"]
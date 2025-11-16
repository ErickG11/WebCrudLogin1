# ===== STAGE 1: build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todo el código
COPY . .

# Restaura paquetes
RUN dotnet restore

# Publica en modo Release
RUN dotnet publish -c Release -o /app

# ===== STAGE 2: runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia lo publicado
COPY --from=build /app ./

# Render detecta el puerto automáticamente; usamos 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# OJO: el nombre del dll debe ser el de tu proyecto
ENTRYPOINT ["dotnet", "WebCrudLogin.dll"]

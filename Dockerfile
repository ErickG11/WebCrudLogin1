# 1) Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todo el código
COPY . .

# Restaurar paquetes
RUN dotnet restore

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# 2) Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copiar artefactos publicados
COPY --from=build /app/publish ./

# Exponer puerto 10000 (el que usaremos en Render)
EXPOSE 10000

# Lanzar la aplicación en ese puerto
CMD ["dotnet", "WebCrudLogin.dll", "--urls", "http://0.0.0.0:10000"]

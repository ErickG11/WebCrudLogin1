# ===== STAGE 1: build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el repo
COPY . .

# Restauramos solo el proyecto web
RUN dotnet restore "WebCrudLogin/WebCrudLogin.csproj"

# Publicamos el proyecto web en modo Release
RUN dotnet publish "WebCrudLogin/WebCrudLogin.csproj" -c Release -o /app

# ===== STAGE 2: runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos lo publicado desde la imagen de build
COPY --from=build /app ./

# Render detecta el puerto automáticamente; usamos 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Ejecutamos la app
ENTRYPOINT ["dotnet", "WebCrudLogin.dll"]

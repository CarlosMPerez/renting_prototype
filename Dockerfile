# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos sólo los proyectos necesarios para la API
COPY src/RentingPrototype.Api/RentingPrototype.Api.csproj src/RentingPrototype.Api/
COPY src/RentingPrototype.Application/RentingPrototype.Application.csproj src/RentingPrototype.Application/
COPY src/RentingPrototype.Domain/RentingPrototype.Domain.csproj src/RentingPrototype.Domain/
COPY src/RentingPrototype.Infrastructure/RentingPrototype.Infrastructure.csproj src/RentingPrototype.Infrastructure/

RUN dotnet restore src/RentingPrototype.Api/RentingPrototype.Api.csproj

# Copiamos el resto del código
COPY . .

# Publicamos la API
RUN dotnet publish src/RentingPrototype.Api/RentingPrototype.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RentingPrototype.Api.dll"]

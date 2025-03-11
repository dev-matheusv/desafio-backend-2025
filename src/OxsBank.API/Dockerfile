# Usando a imagem oficial do .NET SDK para build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Usando a imagem oficial do .NET SDK para compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OxsBank.API/OxsBank.API.csproj", "OxsBank.API/"]
COPY ["OxsBank.Application/OxsBank.Application.csproj", "OxsBank.Application/"]
COPY ["OxsBank.Domain/OxsBank.Domain.csproj", "OxsBank.Domain/"]
COPY ["OxsBank.Infrastructure/OxsBank.Infrastructure.csproj", "OxsBank.Infrastructure/"]
RUN dotnet restore "OxsBank.API/OxsBank.API.csproj"

# Copia os arquivos e compila a aplicação
COPY . .
WORKDIR "/src/OxsBank.API"
RUN dotnet publish -c Release -o /app/publish

# Executa a aplicação
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OxsBank.API.dll"]

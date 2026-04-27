FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Services/Sac/Sac.Api/Sac.Api.csproj", "Services/Sac/Sac.Api/"]
COPY ["Services/Sac/Sac.Application/Sac.Application.csproj", "Services/Sac/Sac.Application/"]
COPY ["Services/Sac/Sac.Domain/Sac.Domain.csproj", "Services/Sac/Sac.Domain/"]
COPY ["Services/Sac/Sac.Infrastructure/Sac.Infrastructure.csproj", "Services/Sac/Sac.Infrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]

RUN dotnet restore "Services/Sac/Sac.Api/Sac.Api.csproj"
COPY . .

WORKDIR "/src/Services/Sac/Sac.Api"
RUN dotnet build "Sac.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sac.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sac.Api.dll"]

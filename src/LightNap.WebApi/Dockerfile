# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files first for better layer caching
COPY ["LightNap.Core/LightNap.Core.csproj", "LightNap.Core/"]
COPY ["LightNap.DataProviders.Sqlite/LightNap.DataProviders.Sqlite.csproj", "LightNap.DataProviders.Sqlite/"]
COPY ["LightNap.DataProviders.SqlServer/LightNap.DataProviders.SqlServer.csproj", "LightNap.DataProviders.SqlServer/"]
COPY ["LightNap.DataProviders.PostgreSQL/LightNap.DataProviders.PostgreSQL.csproj", "LightNap.DataProviders.PostgreSQL/"]
COPY ["LightNap.WebApi/LightNap.WebApi.csproj", "LightNap.WebApi/"]

# Restore dependencies
RUN dotnet restore "./LightNap.WebApi/LightNap.WebApi.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/LightNap.WebApi"
RUN dotnet build "./LightNap.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LightNap.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightNap.WebApi.dll"]
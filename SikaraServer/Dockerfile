# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8040


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SikaraServer/SilkaraServer.csproj", "SikaraServer/"]
COPY ["JsonManagerLib/JsonManagerLib.csproj", "JsonManagerLib/"]
COPY ["UtpTypes/UtpTypes.csproj", "UtpTypes/"]
COPY ["UTP/UTP.csproj", "UTP/"]
RUN dotnet restore "./SikaraServer/SilkaraServer.csproj"
COPY . .
WORKDIR "/src/SikaraServer"
RUN dotnet build "./SilkaraServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SilkaraServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SilkaraServer.dll"]

# Healthcheck
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 CMD curl -f http://localhost:8040/health || exit 1
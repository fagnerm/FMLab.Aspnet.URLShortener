# API - Layered architecture boilerplate
# Copyright (c) 2026 Fagner Marinho
# Licensed under the MIT License. See LICENSE file in the project root for details.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first to leverage Docker layer caching for restore
COPY ["FMLab.Aspnet.URLShortener.Api/FMLab.Aspnet.URLShortener.Api.csproj", "FMLab.Aspnet.URLShortener.Api/"]
COPY ["FMLab.Aspnet.URLShortener.Business/FMLab.Aspnet.URLShortener.Business.csproj", "FMLab.Aspnet.URLShortener.Business/"]
COPY ["FMLab.Aspnet.URLShortener.Infrastructure/FMLab.Aspnet.URLShortener.Infrastructure.csproj", "FMLab.Aspnet.URLShortener.Infrastructure/"]
RUN dotnet restore "FMLab.Aspnet.URLShortener.Api/FMLab.Aspnet.URLShortener.Api.csproj"

COPY . .
WORKDIR "/src/FMLab.Aspnet.URLShortener.Api"
RUN dotnet build "FMLab.Aspnet.URLShortener.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FMLab.Aspnet.URLShortener.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FMLab.Aspnet.URLShortener.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["anime-site/anime-site.csproj", "anime-site/"]
COPY ["AnimeSite.Core/AnimeSite.Core.csproj", "AnimeSite.Core/"]
COPY ["AnimeSite.DataAccess/AnimeSite.DataAccess.csproj", "AnimeSite.DataAccess/"]
RUN dotnet restore "anime-site/anime-site.csproj"
COPY . .
WORKDIR "/src/anime-site"
RUN dotnet build "anime-site.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "anime-site.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "anime-site.dll"]

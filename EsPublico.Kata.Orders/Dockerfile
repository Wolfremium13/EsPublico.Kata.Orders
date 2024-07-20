FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EsPublico.Kata.Orders/EsPublico.Kata.Orders.csproj", "EsPublico.Kata.Orders/"]
RUN dotnet restore "EsPublico.Kata.Orders/EsPublico.Kata.Orders.csproj"
COPY . .
WORKDIR "/src/EsPublico.Kata.Orders"
RUN dotnet build "EsPublico.Kata.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EsPublico.Kata.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EsPublico.Kata.Orders.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Создаем папку uploads с полными правами
# 777 = чтение/запись/выполнение для всех
RUN mkdir -p /app/uploads && chmod 777 /app/uploads

EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Development

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["LokovApp/LokovApp.csproj", "LokovApp/"]
RUN dotnet restore "LokovApp/LokovApp.csproj"
COPY . .
WORKDIR "/src/LokovApp"
RUN dotnet build "LokovApp.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "LokovApp.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Финальное создание папки с правами
RUN mkdir -p /app/uploads && chmod 777 /app/uploads

USER app
ENTRYPOINT ["dotnet", "LokovApp.dll"]
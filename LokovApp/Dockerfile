FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# EXPOSE 5001

# ENV ASPNETCORE_URLS=http://+:5001

USER app
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
ENTRYPOINT ["dotnet", "LokovApp.dll"]

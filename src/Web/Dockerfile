FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY ["src/Web/Web.csproj", "Web/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "Web/Web.csproj"

# Copy everything else
COPY ["src/", "./"]

# Build and publish
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConnectFlow.Web.dll"]

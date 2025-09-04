# ------------------------------
# Stage 1: Base runtime image
# ------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ------------------------------
# Stage 2: Build and restore
# ------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["CurrencyConverter.Api/CurrencyConverter.Api.csproj", "CurrencyConverter.Api/"]
COPY ["CurrencyConverter.Application/CurrencyConverter.Application.csproj", "CurrencyConverter.Application/"]
COPY ["CurrencyConverter.Domain/CurrencyConverter.Domain.csproj", "CurrencyConverter.Domain/"]
COPY ["CurrencyConverter.Infrastructure/CurrencyConverter.Infrastructure.csproj", "CurrencyConverter.Infrastructure/"]

RUN dotnet restore "CurrencyConverter.Api/CurrencyConverter.Api.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/CurrencyConverter.Api"
RUN dotnet build "CurrencyConverter.Api.csproj" -c Release -o /app/build

# ------------------------------
# Stage 3: Publish
# ------------------------------
FROM build AS publish
RUN dotnet publish "CurrencyConverter.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ------------------------------
# Stage 4: Final runtime image
# ------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment is injected at runtime: Development / Test / Production
# Example: docker run -e DOTNET_ENVIRONMENT=Development ...
ENTRYPOINT ["dotnet", "CurrencyConverter.Api.dll"]

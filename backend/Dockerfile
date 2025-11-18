# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY src/WMS.sln ./
COPY src/WMS.API/WMS.API.csproj ./WMS.API/
COPY src/WMS.Domain/WMS.Domain.csproj ./WMS.Domain/
COPY src/WMS.Application/WMS.Application.csproj ./WMS.Application/
COPY src/WMS.Infrastructure/WMS.Infrastructure.csproj ./WMS.Infrastructure/
COPY src/WMS.Shared/WMS.Shared.csproj ./WMS.Shared/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ ./

# Build the application
WORKDIR /src/WMS.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "WMS.API.dll"]

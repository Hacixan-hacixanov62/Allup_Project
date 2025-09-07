# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Allup_Project/Allup_Project.csproj", "Allup_Project/"]
COPY ["Allup_DataAccess/Allup_DataAccess.csproj", "Allup_DataAccess/"]
COPY ["Allup_Core/Allup_Core.csproj", "Allup_Core/"]
COPY ["Allup_Service/Allup_Service.csproj", "Allup_Service/"]
RUN dotnet restore "Allup_Project/Allup_Project.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Allup_Project"
RUN dotnet build "Allup_Project.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Allup_Project.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Allup_Project.dll"]

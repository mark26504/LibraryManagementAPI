# ==============================
# Stage 1: Build
# ==============================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files
COPY ["LibraryManagement.API/LibraryManagement.API.csproj", "LibraryManagement.API/"]

COPY ["Core/Domain/LibraryManagement.Domain.csproj", "Core/Domain/"]
COPY ["Core/Services.Abstraction/LibraryManagement.Services.Abstraction.csproj", "Core/Services.Abstraction/"]
COPY ["Core/Services/LibraryManagement.Services.csproj", "Core/Services/"]

COPY ["Infrastructure/Presentation/LibraryManagement.Presentation.csproj", "Infrastructure/Presentation/"]
COPY ["Infrastructure/Presistence/LibraryManagement.Persistence.csproj", "Infrastructure/Presistence/"]

COPY ["Shared/LibraryManagement.Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "LibraryManagement.API/LibraryManagement.API.csproj"

# Copy all source files
COPY . .

# Publish application
RUN dotnet publish "LibraryManagement.API/LibraryManagement.API.csproj" -c Release -o /app/publish --no-restore


# ==============================
# Stage 2: Runtime
# ==============================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Prepare writable directories for the non-root user
RUN mkdir -p /app/wwwroot/uploads/books \
    /home/app/.aspnet/DataProtection-Keys \
    /app/logs \
    && chown -R app:app /app/wwwroot/uploads/books \
    /home/app/.aspnet \
    /app/logs \
    && chmod 700 /home/app/.aspnet/DataProtection-Keys

ENV ASPNETCORE_HTTP_PORTS=8080
ENV HOME=/home/app

EXPOSE 8080

# Run the application as a non-root user
USER $APP_UID

ENTRYPOINT ["dotnet", "LibraryManagement.API.dll"]
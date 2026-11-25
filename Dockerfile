
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

RUN mkdir -p /app/wwwroot/uploads/books

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "LibraryManagement.API.dll"]
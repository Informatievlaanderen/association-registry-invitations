# Use the .NET SDK as base image
FROM mcr.microsoft.com/dotnet/sdk:6.0.201 AS build
WORKDIR /app

# Copy global.json
COPY global.json .

# Copy the tool manifest and restore tools
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore

# Copy just the paket.dependencies and paket.lock first for caching purposes
COPY ./paket.dependencies ./
COPY ./paket.lock ./

# Restore the Paket dependencies
RUN dotnet paket restore

# Restore csproj dependencies and build
COPY *.sln .

COPY src/AssociationRegistry.Invitations.Api/*.csproj ./src/AssociationRegistry.Invitations.Api/
COPY src/AssociationRegistry.Invitations.Api/paket.references ./src/AssociationRegistry.Invitations.Api/

COPY test/AssociationRegistry.Invitations.Api.Tests/*.csproj ./test/AssociationRegistry.Invitations.Api.Tests/
COPY test/AssociationRegistry.Invitations.Api.Tests/paket.references ./test/AssociationRegistry.Invitations.Api.Tests/

RUN dotnet restore

# Copy all other source files and build
COPY . .

RUN dotnet publish test/AssociationRegistry.Invitations.Api.Tests -c Release -o /app/publish

ENTRYPOINT ["dotnet", "test", "/app/publish/AssociationRegistry.Invitations.Api.Tests.dll", "--logger", "html"]
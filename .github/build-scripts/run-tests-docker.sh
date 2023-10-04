dotnet tool restore; 
dotnet paket restore;

dotnet restore test/AssociationRegistry.Invitations.Api.Tests --runtime linux-x64
dotnet build --no-restore --runtime 'linux-x64' test/AssociationRegistry.Invitations.Api.Tests
dotnet test test/AssociationRegistry.Invitations.Api.Tests --logger html
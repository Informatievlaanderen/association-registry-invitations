dotnet tool restore; 
dotnet paket restore -v;

dotnet restore test/AssociationRegistry.Invitations.Archiver.Tests --runtime linux-x64
dotnet build --no-restore --runtime 'linux-x64' test/AssociationRegistry.Invitations.Archiver.Tests
dotnet test test/AssociationRegistry.Invitations.Archiver.Tests --logger html

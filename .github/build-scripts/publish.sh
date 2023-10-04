dotnet tool restore; 
dotnet paket restore;

dotnet restore src/AssociationRegistry.Invitations.Api --runtime linux-x64

dotnet build --no-restore --runtime 'linux-x64' --self-contained -f net6.0 src/AssociationRegistry.Invitations.Api
dotnet publish -o dist/AssociationRegistry.Invitations.Api/linux --no-build --no-restore --runtime 'linux-x64' --self-contained -f net6.0 src/AssociationRegistry.Invitations.Api


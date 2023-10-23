dotnet tool restore; 
dotnet paket restore -v;

dotnet restore test/$1 --runtime linux-x64
dotnet build --no-restore --runtime 'linux-x64' test/$1
dotnet test test/$1 --logger html

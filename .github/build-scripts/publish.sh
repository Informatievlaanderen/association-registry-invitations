dotnet tool restore; 
dotnet paket restore;

dotnet restore src/$1 --runtime linux-x64

dotnet build -c Release --no-restore --runtime 'linux-x64' --self-contained -f net8.0 src/$1
cd src/$1
dotnet publish -c Release -o ../../dist/$1/linux --no-build --no-restore --runtime 'linux-x64' --self-contained -f net8.0
cd ../../

#dotnet publish -c Release -o dist/$1/linux --no-build --no-restore --runtime 'linux-x64' --self-contained -f net8.0 src/$1


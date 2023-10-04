# Set up the docker build command
dockerRegistry="${BUILD_DOCKER_REGISTRY:-dev.local}"
dockerRepository="association-registry"
containerName="invitations-api"
buildNumber="${CI_BUILD_NUMBER:-0.0.0}"
project="AssociationRegistry.Invitations.Api"

docker_cmd="docker build . --no-cache --tag $dockerRegistry/$dockerRepository/$containerName:$buildNumber --build-arg build_number=$buildNumber"

# Go to dist folder
cd "dist/AssociationRegistry.Invitations.Api/linux" || exit

# Run the docker build command, and storeit's result
$docker_cmd 
result=$?

if [ $? -eq 0 ]; then
    echo "Docker build succeeded!"
else
    echo "Docker build failed or timed out!"
fi

exit $result
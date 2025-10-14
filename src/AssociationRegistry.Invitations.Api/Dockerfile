FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.21

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
COPY / /app

WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
     chmod +x ./init.sh

EXPOSE 11009/tcp

ENV ASPNETCORE_URLS http://*:11009

ENTRYPOINT ["./init.sh"]

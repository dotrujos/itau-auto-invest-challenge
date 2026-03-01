FROM mcr.microsoft.com/devcontainers/dotnet:10.0

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
    mysql-client \
    curl

RUN dotnet tool install --global dotnet-ef --version 9.* \
    && dotnet tool install --global dotnet-counters \
    && dotnet tool install --global dotnet-trace

ENV PATH="$PATH:/root/.dotnet/tools"

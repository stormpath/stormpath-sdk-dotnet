FROM microsoft/dotnet:latest

MAINTAINER Nate Barbettini "nate@stormpath.com"

COPY ../../. /app
WORKDIR /app/test/Stormpath.SDK.CoreClr

# Skip sync tests
#RUN rm -rf Sync

RUN dotnet restore
RUN dotnet build

ENV STORMPATH_IT_FASTMODE true
ENTRYPOINT dotnet 
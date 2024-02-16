FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
WORKDIR /hub

RUN dotnet workload install wasm-tools

COPY hub.sln .
COPY Client/hub.Client.csproj Client/hub.Client.csproj
COPY Server/hub.Server.csproj Server/hub.Server.csproj
COPY Shared/hub.Shared.csproj Shared/hub.Shared.csproj
COPY .nuke .nuke
COPY Build Build
COPY build.sh .

RUN ./build.sh restore

COPY . .

RUN ./build.sh publish 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runner
WORKDIR /hub

COPY --from=builder /hub/Client/bin/Release/net8.0/publish ./Client
COPY --from=builder /hub/Server/bin/Release/net8.0/publish ./Server

WORKDIR /hub/Client

ENTRYPOINT [ "dotnet", "/hub/Server/hub.Server.dll"]

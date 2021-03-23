FROM mcr.microsoft.com/dotnet/core/sdk:3.1.402-alpine3.12 as builder
WORKDIR /dotvote

COPY dotvote.sln .
COPY Client/dotvote.Client.csproj Client/dotvote.Client.csproj
COPY Server/dotvote.Server.csproj Server/dotvote.Server.csproj
COPY Shared/dotvote.Shared.csproj Shared/dotvote.Shared.csproj

RUN dotnet restore dotvote.sln

COPY . .

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.8-alpine3.12 as runner
WORKDIR /dotvote

COPY --from=builder /dotvote/Client/bin/Release/netstandard2.1/publish ./Client
COPY --from=builder /dotvote/Server/bin/Release/netcoreapp3.1/publish ./Server

WORKDIR /dotvote/Client

ENTRYPOINT [ "dotnet", "/dotvote/Server/dotvote.Server.dll"]

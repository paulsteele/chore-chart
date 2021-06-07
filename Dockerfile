FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /chores

COPY chores.sln .
COPY Client/chores.Client.csproj Client/chores.Client.csproj
COPY Server/chores.Server.csproj Server/chores.Server.csproj
COPY Shared/chores.Shared.csproj Shared/chores.Shared.csproj

RUN dotnet restore chores.sln

COPY . .

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runner
WORKDIR /chores

COPY --from=builder /chores/Client/bin/Release/net5.0/publish ./Client
COPY --from=builder /chores/Server/bin/Release/net5.0/publish ./Server

WORKDIR /chores/Client

ENTRYPOINT [ "dotnet", "/chores/Server/chores.Server.dll"]

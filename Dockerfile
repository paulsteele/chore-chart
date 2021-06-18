FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /home

COPY chores.sln .
COPY Client/home.Client.csproj Client/home.Client.csproj
COPY Server/home.Server.csproj Server/home.Server.csproj
COPY Shared/home.Shared.csproj Shared/home.Shared.csproj

RUN dotnet restore home.sln

COPY . .

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runner
WORKDIR /home

COPY --from=builder /home/Client/bin/Release/net5.0/publish ./Client
COPY --from=builder /home/Server/bin/Release/net5.0/publish ./Server

WORKDIR /home/Client

ENTRYPOINT [ "dotnet", "/home/Server/home.Server.dll"]

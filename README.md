# hub
An application to facilitate home life

# Dependencies to install
* [Dotnet 5](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
* [Podman](https://podman.io/getting-started/)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)
    * install the cli so calling `dotnet ef` is a valid command

# Development
This project is created using [Blazor](https://dotnet.microsoft.com/learn/aspnet/blazor-tutorial/install)

It utilizes an asp.net backend and a WASM frontend with shared models between each side
## Running Database Locally
* Ensure podman is running
* first time
    * `./build.sh createDevContainers`
* other times
    * `./build.sh startDevContainers`
* stopping
    * `./build.sh stopDevContainers`
* deleting
    * `/build.sh removeDevContainers`
    * for testing fresh databases

## Running the App Locally
* `./build.sh run`

## Database Migrations
* Listing Migrations
    * `./build.sh listMigrations`

* Adding a Migration
    * `./build.sh addMigration`

## Errors
```      
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure()' to the 'UseMySql' call.
       ---> MySql.Data.MySqlClient.MySqlException (0x80004005): Unable to connect to any of the specified MySQL hosts.
```
* The database is not running / reachable on default port

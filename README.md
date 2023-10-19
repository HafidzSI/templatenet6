 # NetCa

## Technologies
* .NET 6
* ASP .NET 6
* Entity Framework Core 6
* [DDD and CQRS Patterns](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

## Getting Started

The easiest way to get started is

1. Install the latest [.NET SDK](https://dotnet.microsoft.com/download)
2. run `git clone https://devops.unitedtractors.com/DefaultCollection/Mobile%20Web%20Development/_git/NetCa` to clone the project
2. Navigate to `src/Api` and run `dotnet restore` to restore dependencies
3. run `dotnet run` to launch the project

### Domain

This will contain all entities, enums, exceptions, types and logic specific to the domain.
The Entity Framework related classes are abstract, and should be considered in the same light as .NET Core.
For testing, use an InMemory provider such as InMemory or SqlLite.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### Persistance

This layer is a group of files which is used to communicate between the application and DB.
Fix issue on [ef core migrations](https://stackoverflow.com/questions/56862089/cannot-find-command-dotnet-ef) and [Migration in different assembly ](https://github.com/aspnet/EntityFrameworkCore/issues/5900)

``` bash
cd ../Persistance
dotnet ef --startup-project ../Api/ migrations add Initital
```

### Api

This layer is a REST API. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

### Testing

``` bash
dotnet test <name-service>.sln
```

## Source

This project base on [JasonGT](https://github.com/JasonGT/CleanArchitecture)

## License

This project is licensed with the [MIT license](LICENSE.md).

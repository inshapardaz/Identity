# Identity
Authentication server for inshapardaz

# Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/hmtcrynhyxbtxdno?svg=true)](https://ci.appveyor.com/project/umerfaruk/identity)

# Components

1. Indentity Server
2. Database

## Tools and Frameworks requires
- Dotnet Core (https://www.microsoft.com/net/core)
- SQL Server 2016

### Rest Service and WebSite
Checkout code and 

`dotnet restore`

`dotnet build`

`dotnet run`


### Adding migrations for Identity Server

[Source](http://docs.identityserver.io/en/release/quickstarts/8_entity_framework.html#adding-migrations)

`dotnet ef migrations add ApplicationDbMigration -c ApplicationDbContext -o Data/Migrations/Application/ApplicationDb`    

`dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb`

`dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb`    



#### Update database

'dotnet ef database update -c ApplicationDbMigration'

`dotnet ef database update -c PersistedGrantDbContext`

'dotnet ef database update -c ConfigurationDbContext'


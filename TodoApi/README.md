# Tutorial codes for TodoApi

## Overview

When this is running, open https://localhost:5001/swagger. 

Use the following to create a new user:
- For ease of testing purpose, this does not require authentication.
- Use "POST api/Users" API

Use the following to delete a user:
- For ease of testing purpose, this does not require authentication.
- Use "DELETE api/Users/{id}" API

Then you can use 1 of the following to authenticate:
- Use "POST api/Users/authenticate/cookie" API to authenticate and use "POST api/Users/signout/cookie" to sign out.
- Use "POST api/Users/authenticate/jwt" API and Swagger's Authenticate button to authenticate.

You can find the added codes for each of the topics below in each commit.

## Initial tutorial

- https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio-code

## Dependency injection

- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0

## JWT bearer token auth

- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0
- https://www.c-sharpcorner.com/article/asp-net-core-web-api-5-0-authentication-using-jwtjson-base-token/
   - dotnet add package Microsoft.AspNetCore.Authentication
   - dotnet add package System.IdentityModel.Tokens.Jwt
   - dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
- (Check the Claim part.) https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
- (Check the Swagger part.) https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-5-with-jwt-and-swagger/

## Logging in ASP.NET Core

- https://docs.microsoft.com/en-us/dotnet/core/extensions/logging-providers
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0

## Cookie auth

- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
   - https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-5.0&tabs=aspnetcore2x
- https://www.c-sharpcorner.com/article/cookie-authentication-in-asp-net-core/ 

## Swashbuckle

- https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio-code
- https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-5.0
- https://medium.com/@jrhodes.home/exposing-enums-through-swagger-in-net-core-api-616d3727a02c

## Identity

### Basic

- Use Entity Framework Core and migration to create the basic identiy database tables. Then new user can be created and then logged in.
- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/individual?view=aspnetcore-5.0
- https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-web-api-with-json-web-tokens/
   - dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   - dotnet add package Microsoft.EntityFrameworkCore.Tools
   - dotnet add package Microsoft.AspNetCore.Identity
   - dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   - dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

### Advanced

- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-5.0
- https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0

## Useful .NET CLI commands

- dotnet run --configuration Release

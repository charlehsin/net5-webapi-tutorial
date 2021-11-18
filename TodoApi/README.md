# Tutorial and sample codes for Web API

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

An extra authentication flow to use Facebook OAuth 2.0 provider is provided.
- Use "GET api/Users/authenticate/facebook" API to challenge Facebook.
   - Redirection (302) will be returned.
   - However, Swagger UI will not do the redirection because its CORS policy will block redirecting to Facebook. You need to manually get the location response header via web browser's developer tool, and open a browser tab at that URL to log into Facebook.
   - After that, the JWT will be returned at the redirection page tab. For tutorial purpose, the Facebook user is fixed to have Admin role.

In the sample codes, we use 2 different DbContext for tutorial purpose:
- For TodoItems, we use in-memory database.
- For Users, we use Sqlite.

You can find the added codes for each of the topics below in each commit.

## Folder structure

- Authentication folder: Handle JWT, auth cookie, and claim.
- Controllers folder: API controllers
- Data folder: Data context class for users.
   - Migrations folder: Use Entity Framework Core to create the Sqlite database tables.
- Identity folder: Define the AppUser class for Entity Framework Core, and define wrappers for ASP Identity's UserManager and RoleManager.
- Models folder: Define some data classes used by API.
- Repositories folder: Handle the storage operations. For users, we do not create repository directly since we are using ASP Identity.

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
- (More details but in ASP.NET 4.) https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api

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

- Use Entity Framework Core and migration to create the basic identity database tables. Then new user can be created and then logged in.
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

## External OAuth 2.0 provider

- The sample codes will use Facebook for tutorial purpose.
- We keep the APIs (except for authentication-type APIs) to use JWT authentication and cookie authentication. Therefore, when the authentication via Facebook is done, we will return a JWT.
   - Different design can be used here. For example, you can configure API to use Facebook authentication scheme directly (instead of JWT or cookie). In this case, once the authentication via Facebook is done, the API is authenticated.
      - Check https://docs.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-6.0#extend-or-add-custom-claims-using-iclaimstransformation, if you need to add other claims, e.g., the role claim.
      - By default, if the authentication is not done yet, Redirection (302) will be returned, instead of Unauthorized (401), for the APIs using Facebook authentication scheme. You may want to change this behavior to return Unauthorized (401) instead. You probably don't want to let non-authentication-type of APIs to handle the authentication redirection request.
- General concept
   - https://www.digitalocean.com/community/tutorials/an-introduction-to-oauth-2
   - https://andrewlock.net/an-introduction-to-oauth-2-using-facebook-in-asp-net-core/
   - https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0
   - https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows
- Coding guide
   - https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-5.0&tabs=visual-studio-code
      - dotnet add package Microsoft.AspNetCore.Authentication.Facebook --version 5.0.12
      - Enable secret storage for development
         - dotnet user-secrets init
      - Add the Facebook secrets
         - dotnet user-secrets set "Authentication:Facebook:AppId" "<app-id>"
         - dotnet user-secrets set "Authentication:Facebook:AppSecret" "<app-secret>"
      - List the secrets
         - dotnet user-secrets list
      - Remove the secret
         - dotnet user-secrets remove "Authentication:Facebook:AppId"
         - dotnet user-secrets remove "Authentication:Facebook:AppSecret"
   - https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationhttpcontextextensions.authenticateasync?view=aspnetcore-5.0
   - https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationhttpcontextextensions.challengeasync?view=aspnetcore-5.0

## Useful .NET CLI commands

- dotnet run --configuration Release

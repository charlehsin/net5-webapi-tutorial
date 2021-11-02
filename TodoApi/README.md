# Tutorial codes for TodoApi

## Overview

When this is running, open https://localhost:5001/swagger. Then you can use 1 of the following to authenticate:
- Use "POST api/Users/authenticate/cookie" API to authenticate and use "POST api/Users/signout/cookie" to sign out.
- Use "POST api/Users/authenticate/jwt" API and Swagger's Authenticate button to authenticate.

Currently, for this tutorial purpose, to authenticate, you need to use "tester" as the user name and "P@ss0wrd" as the password.
You should not use this for production.

## Initial tutorial

- commit 0fdf741fbae4b9706bd048732307b3c152f0ed92
- https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio-code

## Dependency injection

- commit 688e90f393c851fc3dbe5fa2e68d11e8400c5e45
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0

## JWT bearer token auth

- commit 341c847a152f1651d47fac810e6dcd18f571c76c
- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0
- https://www.c-sharpcorner.com/article/asp-net-core-web-api-5-0-authentication-using-jwtjson-base-token/
   - dotnet add package Microsoft.AspNetCore.Authentication
   - dotnet add package System.IdentityModel.Tokens.Jwt
   - dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
- (Check the Claim part.) https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
- (Check the Swagger part.) https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-5-with-jwt-and-swagger/

## Logging in ASP.NET Core

- commit 3265786f649dda1bb9168d05e84d2ed051b6236e
- https://docs.microsoft.com/en-us/dotnet/core/extensions/logging-providers
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0

## Cookie auth

- commit 113b690de388c0802a6f0727e22120a08e0a0bd8
- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
   - https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-5.0&tabs=aspnetcore2x
- https://www.c-sharpcorner.com/article/cookie-authentication-in-asp-net-core/ 

## User framework

## Useful .NET CLI commands

- dotnet run --configuration Release

# Tutorial codes for unit testing for TodoApi

## Basic unit testing 

- commit 0fdf741fbae4b9706bd048732307b3c152f0ed92
- https://docs.microsoft.com/en-us/dotnet/core/tutorials/testing-library-with-visual-studio-code
- https://www.pluralsight.com/guides/testing-.net-core-apps-with-visual-studio-code

## Mock

- commit 57ae2c18eee9e450c98552454f481652dd2848d5
- commit 688e90f393c851fc3dbe5fa2e68d11e8400c5e45
- https://www.c-sharpcorner.com/article/moq-unit-test-net-core-app-using-mock-object/
- https://github.com/moq/moq4/tree/fc484fb85
- https://github.com/Moq/moq4/wiki/Quickstart 
- https://stackoverflow.com/questions/28581322/moq-with-task-await 

## ASP related

- commit 688e90f393c851fc3dbe5fa2e68d11e8400c5e45 
- https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api

## .NET CLI

- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test
- https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=mstest

## Useful .NET CLI commands

- dotnet test
- (test project) dotnet test TodoApi.Tests.csproj
- (test target method) dotnet test --filter GetTodoItemsAsync
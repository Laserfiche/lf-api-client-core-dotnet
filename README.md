# lf-oauth-api-client-dotnet
.NET implementation of various OAuth 2.0 flows for secure and easy access to Laserfiche APIs.

## Overview
- src contains the implementation of the OAuth API Client.
- tests/integration contains integration tests. You can run it locally with a JSON file that contains the following fields: 
  - baseAddress
  - clientId
  - servicePrincipalKey
  - signingKey
  You could also just run with GitHhub actions and the JSON file is handled automatically.
- tests/unit contains unit tests. You can run them locally without any setup. It will also be run when you make a PR.

For more detailed documentation, see the Laserfiche Developer [site](https://developer.laserfiche.com/guide_oauth-service.html).

## How to contribute
Technically you could use any editors you like. But it's more convenient if you are using either Visual Studio Code or Visual Studio. Here is a few useful commands for building and testing the app.

### Build
`dotnet build --no-restore`

### Release Build
`dotnet build --configuration Release --no-restore`

### Make a Nuget Package
`dotnet pack` (in the same directly where the `.csproj` file resides)

### Add a Nuget Pacakge in a Local Directory
`dotnet add package {package_id} --source {absolute_path_to_folder_containing_nupkg}`

### Run Unit Tests
`dotnet test --verbosity normal --filter Laserfiche.OAuth.Client.ClientCredentials.UnitTest`

### Run Integration Tests
`dotnet test --verbosity normal --filter Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest`

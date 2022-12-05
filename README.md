# Laserfiche API Client Core .NET
.NET implementation of various foundational APIs for Laserfiche, including authorization APIs such as OAuth 2.0 flows for secure and easy access to Laserfiche APIs.

Documentation [Laserfiche OAuth 2.0 Authorization Server API](https://developer.laserfiche.com/API.html).

## Overview
- `src` contains the implementation of the foundational authentication/authorization related code for APIs of various Laserfiche products.
- `src\APIServer` contains all self-hosted API Server authentication/authorization related client code.
- `src\HttpHandlers` contains a client implementation of OAuth 2.0 Client Credentials Flow.
- `src\OAuth` contains all OAuth related client code.
- `src\Utils` contains all utility functions and classes for Laserfiche APIs.
- `tests\integration` contains all integration tests. To run them, you either use GitHub Workflows, or you could provide the `.env` files in your file system and run them there.
- `test\unit` contains all unit tests.

## How to contribute
Technically you could use any editors you like. But it's more convenient if you are using either Visual Studio Code or Visual Studio. Here is a few useful commands for building and testing the app.

### Generate the oauth client
1. Download the `nswag` command line tool. Instructions can be found [here](https://github.com/RicoSuter/NSwag/wiki/CommandLine).
2. From the root directory of this Git repository, run the command `nswag run oauth-nswag.json`


### Build, Test, and Package

See the [.github/workflows/main.yml](.github/workflows/main.yml).

### Change Log

See CHANGELOG [here](https://github.com/Laserfiche/lf-api-client-core-dotnet/blob/HEAD/CHANGELOG.md).
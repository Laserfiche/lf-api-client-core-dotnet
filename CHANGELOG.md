# Changelog

## 1.3.4

### Chore & Maintenance

- Update `Microsoft.IdentityModel.JsonWebTokens` to `7.3.1`

## 1.3.2

### Features

- Expose the `JwtUtils` class allowing clients to create an Authorization JWT used to request an Access Token.

## 1.3.1

### Features

- An optional `scope` parameter has been added when requesting an access token for `GetAccessTokenFromCode` and `GetAccessTokenFromServicePrincipalAsync`.

## 1.3.0

### Fixes

- Fix the error message when an `ApiException` is thrown and use the `ProblemDetails.Title` if possible.
- Add more properties to the `ProblemDetails` type to more accurately represent the response.
- **[BREAKING]** Types `ApiException` and `ProblemDetails` have been moved to namespace `Laserfiche.Api.Client`.
- **[BREAKING]** Property `ProblemDetails.AdditionalProperties` has been removed. Use property `ProblemDetails.Extensions` instead.
- **[BREAKING]** Types of `ApiException<T>` has been removed. Use `ApiException` instead. The `ApiException` has a `ProblemDetails` property which may contain additional information.

## 1.2.3

### Fixes

- Change `TargetFramework` to only `netstandard2.0`.
- Lower `Newtonsoft.Json` dependency.
- Remove `System.ComponentModel.Annotations` dependency.
- **[BREAKING]** `DomainUtils`:
  - remove function `GetDomainFromAccountId`.

# Changelog

## 2.0.1

### Features

- Self Hosted `UsernamePasswordHandler` should not try to set the access bearer token when no given username and password.

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

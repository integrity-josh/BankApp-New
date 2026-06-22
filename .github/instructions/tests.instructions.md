```
Description: "This file describes the testing conventions for the BankApp project."
applyTo: '**/BankApp.Tests/**'
```
## Testing Conventions

- Use xUnit, FluentAssertions, and NSubstitute for tests in the `BankApp.Tests` project.
- Tests should work with the public API of the domain and not reach into private fields or methods.
- Follow the existing `When_...` naming style for test classes.
- Prefer arrange/act/assert structure and keep tests focused on one behavior.
- Use tests to verify business rules and edge cases, not just happy paths, and use tests as a blueprint to guide the software design.
- Tests should never be deleted as they are the blueprint for the business rules and software design. If a test is not passing, the software should be fixed, not the test deleted.

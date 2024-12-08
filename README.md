## Solution structure
```
src/
    PaymentGateway.Api
test/
    PaymentGateway.Api.IntegrationTests
    PaymentGateway.Api.UnitTests
```

## Design Considerations
- I utilized .NET validation attributes to validate the request model, simplifying the codebase.
- The solution follows a Clean Architecture-inspired design, with distinct layers. I created use cases to encapsulate business logic and orchestrate application flow, integrating gateways, repositories, and models.

## Test Strategy
- I prioritized integration tests due to the simplicity of the process and the use of built-in .NET features for request validation (which cover a significant portion of the process).
- Unit tests were implemented to validate the business logic in use cases and models. While integration tests fully cover the current use cases, as the application grows and more rules are added, introducing additional unit tests for specific logic would be beneficial.
- I used Bogus to generate realistic fake data for testing.
- A Behavior-Driven Development (BDD) style naming convention (Given_When_Then) was adopted to improve test readability and maintainability.
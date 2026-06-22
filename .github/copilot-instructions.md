# BankApp Project Instructions

This repository is a .NET 10 solution with a thin API layer, a domain layer for business rules, a data layer for persistence, and an xUnit test project. 

## General Guidelines

- Domain driven design principles should be followed, with a clear separation of concerns between the API, domain, and data layers.
- Keep `BankApp.Api` focused on HTTP concerns, MediatR requests/handlers, and response mapping.
- Keep business rules in `BankApp.Domain`; do not move validation or invariants into controllers.
- Prefer MediatR request handlers in `BankApp.Api/Features` for application logic.
- Use `BankAppControllerBase.Execute(...)` for controller actions so API errors stay consistent.
- Pass `CancellationToken` through async handlers and repository calls when available.
- Use `Money` for monetary values in domain logic instead of raw `decimal`.
- Follow SOLID principles and keep descriptively named methods focused on a single responsibility.
- Use repository abstractions and specifications where the code already expects them, especially when loading related data.
- Keep controller routes and feature names aligned with the existing `api/[controller]` and `MakeDeposit` / `MakeWithdrawal` patterns.

## Refactoring

- Don't refactor out duplicate code just to do it, refactor out duplicate business logic. 
- Just having duplicate code is not a problem
  - Duplicating business logic/business operations is BAD
  - Duplicating code is NOT inherently bad

## Guidelines

- Commit with all green tests before refactoring.
- Ask clarifying questions before making changes, especially if the change is not straightforward or if it touches multiple layers.
- If you are unsure about the intent of existing code, ask for clarification rather than making assumptions.
- Prefer small, targeted edits that fit the existing code style.
- Avoid broad refactors, reformatting, or introducing new abstractions unless they solve the current task.
- Do not make any comments in code, only make code changes. If you think a comment is needed, make the code clearer instead with descriptive method and variable names.



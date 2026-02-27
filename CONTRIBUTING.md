# Contributing

This is a solo study and portfolio project by Rodolfo Venancio.
External contributions are not expected, but this file documents
the development standards — both for consistency and as a 
portfolio signal of professional engineering practice.

---

## Decision-first development

Before writing any code for a non-trivial feature:

1. Check if an ADR already covers this decision
2. If not, and the decision is significant — write the ADR first
3. Use `Proposed` status, think through the consequences
4. Then implement

This is not bureaucracy. It is the habit that separates 
engineers who build sustainable systems from those who 
accumulate technical debt.

## Branch strategy
```
main          → stable, deployable state
feat/xxx      → new feature or new API study
study/xxx     → exploration branch (can be messy)
fix/xxx       → bug fix
adr/xxx       → adding or updating an ADR
```

Merge to main only when the feature works end-to-end
and has at least one passing test.

## Commit messages

Follow Conventional Commits:
```
feat: add Proposta endpoint to Minimal API
fix: correct Piso validation logic in NegotiationService  
docs: add ADR-0009 for GraphQL adoption
refactor: extract CatalogueHash to value object
test: add BDD scenarios for identity verification flow
chore: update .NET SDK to 9.0.x
```

The type prefix matters — it makes CHANGELOG generation 
automatable later and communicates intent clearly.

## Code standards

- C# 12+ syntax, .NET 8+ patterns
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- No magic strings — use constants or enums
- No secrets in code — use environment variables or 
  Secret Manager
- Every public method on a Domain class has XML doc comments
- Every new endpoint has at least one integration test
  before the branch is merged

## Testing philosophy

Unit tests for: Domain logic, Value Objects, Guards (PisoGuard)
Integration tests for: API endpoints, database operations
No tests needed for: DTOs, simple mapping, configuration classes

## The Ubiquitous Language rule

All code, comments, variable names, and method names must use
the domain language defined in docs/architecture/:

| Use | Never |
|-----|-------|
| `Proposta` | `Offer`, `Bid` |
| `Piso` | `MinPrice`, `Floor` |
| `Pacotao` | `SyncPayload`, `Bundle` |
| `Anuncio` | `Listing`, `Ad` |
| `PerfilDoComprador` | `UserType`, `BuyerType` |
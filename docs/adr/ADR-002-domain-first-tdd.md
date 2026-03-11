# ADR-002 — Start with Domain-First TDD

## Status
Accepted

## Context

The task includes HTTP endpoints and background processing, but the real complexity sits in the policy rules: capacity, queue admission, overflow, inactivity, shifts, and assignment strategy.

If implementation starts from controllers first, the tests will become noisy too early and the core behavior will be harder to shape.

## Decision

Start implementation with domain-level tests and policy code first, then layer application, API, and worker concerns around it.

## Consequences

### Positive
- important rules become testable early
- refactoring remains safer
- HTTP and timer mechanics stay secondary to business behavior

### Trade-off
- visible API progress comes a little later than in a controller-first approach

# ADR-001 - Use the Latest .NET LTS for the Solution

## Status
Accepted

## Context

The task is being implemented as a backend service with both API and background-processing responsibilities.


## Decision

Use the latest .NET LTS version across:

- Web API
- worker/background services
- domain/application/infrastructure projects
- test projects

## Consequences

### Positive
- available resource and infrastructure goes with .NET
- stable long-term support baseline
- modern runtime and language features
- a realistic choice for a production-minded backend

### Trade-off
- reviewers need the matching SDK installed if they need to run it locally

# 04 - Architecture and Domain Notes

## Design goal

I want the important rules to stay visible, isolated, and testable.

I do not want business behavior hidden inside controllers, timer callbacks, or database code.

## Proposed solution shape

I am leaning toward this structure:

- `src/SupportChat.Api`
- `src/SupportChat.Worker`
- `src/SupportChat.Application`
- `src/SupportChat.Domain`
- `src/SupportChat.Infrastructure`
- `tests/SupportChat.UnitTests`
- `tests/SupportChat.IntegrationTests`

This is still a small solution, but it separates concerns in a way that feels credible for me.  

note: In production grade environment, the `src/SupportChat.Worker` could be under a different solution and a different deployable unit. But we're keeping it under single solution for simplicity purpose only.

## Responsibility by layer

### Domain
Pure business behavior and policies.

Examples:
- capacity calculation
- queue admission policy
- office-hours policy
- shift eligibility
- assignment strategy
- inactivity logic

### Application
Use-case orchestration.

Examples:
- create session
- register poll
- assign waiting session
- mark inactive session

### Infrastructure
Technical concerns.

Examples:
- persistence
- configuration binding
- clock abstraction
- repositories

### API
HTTP contract and request handling.

### Worker
Background processing for assignment and inactivity checks.

## Early domain model notes

The core concepts I expect to model are:

- ChatSession
- Agent
- Team
- ShiftWindow
- QueuePolicy
- AssignmentPolicy
- SessionStatus
- Seniority

## Why this structure fits the task

The task has two things happening at the same time:

1. request-driven actions from the client
2. background-driven actions from the system

That is exactly why I do not want the design to collapse into a controller-and-service blob. The same business rules will be touched from more than one direction.

## Contract-facing workflows

Now that the main API actions are drafted, the architecture is easier to picture through two core flows:

### Intake flow
Client request -> API -> application use case -> queue admission policy -> persistence -> response

### Monitoring / assignment flow
Background worker -> application use case -> policy evaluation -> persistence update

This confirms the earlier direction: the policies should sit in the middle, with API and workers acting as delivery mechanisms around them.


## Trade-offs and Intentional Simplifications
For the purpose of this exercise, a few decisions are intentionally simplified.

The focus of the implementation is correctness of the queueing rules, assignment logic, and session lifecycle rather than infrastructure complexity.

Some examples:

- The worker process is kept in the same solution as the API to keep the project easy to run locally.

- Persistence strategy may start simple (e.g., a relational store without complex scaling strategies).

- Background processing is implemented using hosted services instead of a distributed job system.

- Queue behavior is modeled inside the domain rather than using external messaging systems.

In a production environment, some of these areas could evolve into:

- Independent worker services

- Distributed queue infrastructure

- More advanced observability and scaling strategies

For the scope of this task, keeping these parts simple helps keep the core business rules visible and testable.
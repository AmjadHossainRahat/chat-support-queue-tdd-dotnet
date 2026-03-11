# ADR-003: Use Hosted Services for Background Processing

Status: Accepted  
Date: 2026-03-11

## Context

The system must perform certain actions independent of direct HTTP requests.

Two important examples in this project are:

- assigning queued chat sessions to available agents
- detecting inactive chat sessions when polling stops

These tasks must run continuously in the background while the application is running.

Several implementation approaches are possible:

- external job systems (e.g., Hangfire, Quartz)
- message queues with separate worker services
- .NET background hosted services

Since this project focuses primarily on modeling business rules clearly rather than introducing infrastructure complexity, adding external job systems would introduce operational overhead without providing meaningful benefits for the scope of this exercise.

## Decision

Background processing will be implemented using **.NET Hosted Services (BackgroundService)**.

The worker component will run processes responsible for:

- monitoring queued sessions and assigning them to agents
- detecting inactive sessions based on polling behavior

These background services will call the same application and domain logic used by the API layer to ensure consistent behavior.

## Consequences

Positive:

- simple operational setup
- no external infrastructure required
- background logic remains within the same codebase
- domain rules remain reusable between API and worker processes

Trade-offs:

- less scalable than distributed worker architectures
- background execution tied to the application lifecycle

For the scope of this project, hosted services provide a good balance between simplicity and correctness.
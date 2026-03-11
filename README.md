# Support Chat Queue Management System

This repository contains a backend system that manages support chat sessions, queueing, and agent assignment.

The implementation follows a **domain-first Test Driven Development (TDD)** approach.  
The goal of the project is to model queue capacity, agent shifts, overflow routing, inactivity detection, and seniority-based assignment in a clean and testable way.

## Development Approach

The project follows these principles:

* Domain-first design  
* Test Driven Development (TDD)  
* Clear separation between API, domain logic, and background processing  
* Incremental commits documenting architectural thinking

## Repository Structure

`docs/`  
Design analysis, assumptions, architecture notes, and API contract.

`docs/adr/`  
Architecture Decision Records (ADR).

`src/`  
Application source code (added after documentation phase).

`tests/`  
Unit and integration tests following the TDD approach.


## Current focus

At first I would like to document the problem before I start building the solution. I want the first coding step to happen on top of a clear understanding and well defined asumption.

## Additional Documentations

- `docs/01-task-analysis.md`
- `docs/02-assumptions.md`
- `docs/03-business-rules.md`
- `docs/04-architecture.md`
- `docs/adr/ADR-001-use-latest-dotnet-lts.md`
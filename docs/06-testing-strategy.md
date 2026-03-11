# 06 - Delivery Plan and Testing Strategy

## Why I want TDD here

This task has APIs, but the risky part is not controller wiring. The risky part is getting the queueing and assignment behavior slightly wrong and only noticing it late.

That is why I want to start with tests around business rules first.

## Delivery plan

### Stage 1 — Documentation
Done first so that implementation starts on top of a clear interpretation.

### Stage 2 — Solution skeleton
Create .NET projects for:

- API
- worker
- domain
- application
- infrastructure
- unit tests
- integration tests

### Stage 3 — Domain-first TDD
Build the policy layer with tests around:

- effective capacity
- queue admission
- overflow eligibility
- office-hours behavior
- shift eligibility
- inactivity detection
- junior-first assignment

### Stage 4 — Application use cases
Implement the orchestration for:

- create session
- register poll
- mark inactive session
- assign waiting sessions

### Stage 5 — API and worker integration
Expose the flows over HTTP and run the background processes explicitly.

## Test layers

### Unit tests
For pure rule behavior.

### Application tests
For use-case orchestration.

### Integration tests
For API + persistence + worker interaction.

## What I care most about proving

If I can demonstrate these cleanly, the solution will already look strong:

- queue admission works correctly
- overflow is only used when allowed
- inactive sessions stop consuming live capacity
- off-shift agents do not receive new chats
- lower seniority gets preferred before higher seniority
- FIFO order is respected in waiting flow

# 01 - Task Analysis

What we actually have here is a small support-chat orchestration problem. The API is only the entry point. The interesting part is what happens after a request arrives.

A client starts a support request. The system accepts or refuses it based on queue state. If accepted, the session enters a FIFO waiting flow, is watched for activity through polling, and is later assigned to an eligible agent.

That means the core of the solution is not CRUD. The core is policy and coordination.

## What the task is really asking for

From a business point of view, the system needs to do five things really well:

1. Accept chat sessions into a managed waiting flow.
2. Refuse sessions when queue rules say no.
3. Route excess traffic to overflow only when allowed.
4. Detect when a client has gone silent.
5. Assign waiting sessions to agents using capacity, shift, and seniority rules.

## Requirement breakdown

### Session intake
A request creates a chat session. If the session is accepted, it should be trackable immediately, even if it is not yet assigned.

### Queue and overflow
The main queue is the normal path. Overflow is the fallback path, but not always available. That already tells me this system needs explicit admission logic instead of a simple enqueue-everything approach.

### Polling and inactivity
The task says the client polls every second after receiving OK, and that a session becomes inactive after missing three polls. So heartbeat tracking is part of the business flow, not an optional technical detail.

### Agent assignment
Assignment is not random. It depends on:

- whether the agent is still eligible for new work
- how much effective capacity the agent has
- what shift the agent is in
- a preference toward lower seniority first

### Capacity model
The queue size is tied to team capacity, and team capacity is tied to agent seniority. So capacity cannot be hardcoded as one flat number.

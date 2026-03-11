# ADR-004: Use Polling for Session Liveness Detection

Status: Accepted  
Date: 2026-03-11

## Context

The system needs a mechanism to detect whether a chat session is still active.

According to the task specification, once a session is accepted the client will poll the server periodically.  
If the server does not receive three expected polling requests, the session must be considered inactive.

Several approaches could be used for session liveness detection:

- WebSockets with persistent connections
- Server-Sent Events
- long polling
- periodic client polling

Because the interaction model in this task is request/response oriented and does not require real-time push notifications, introducing persistent connection infrastructure would add unnecessary complexity.

## Decision

Session liveness will be implemented using **periodic HTTP polling** from the client.

The expected behavior is:

- the client sends a poll request approximately every second
- each poll updates the session's last activity timestamp
- if the system detects that three consecutive polling intervals were missed, the session is marked as inactive

This rule will be enforced by a background monitoring process.

## Consequences

Positive:

- simple and predictable interaction model
- no persistent connection management required
- easy to implement and test
- works naturally with stateless HTTP infrastructure

Trade-offs:

- polling generates additional requests
- slightly less efficient than push-based communication mechanisms

For the scope of this project, polling provides a straightforward and reliable way to detect inactive sessions while keeping the system architecture simple.
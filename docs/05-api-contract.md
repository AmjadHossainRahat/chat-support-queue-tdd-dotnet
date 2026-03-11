# 05 - API Contract and Workflow Diagrams

## Intent

This is an initial API contract. I am using it to guide implementation, not to pretend the interface is already frozen forever.

The main thing I want to lock down now is the interaction model:

- create session
- poll session
- inspect session status

## 1) Create chat session

### Endpoint
`POST /api/chat-sessions`

### Purpose
Create a support chat session and try to place it into the intake flow.

### Example request
```json
{
  "customerReference": "CUST-10001",
  "channel": "web"
}
```

### Accepted into main queue
```json
{
  "sessionId": "4e1b3280-7d47-47c7-9a2c-a15ddfbd7cb4",
  "status": "Queued",
  "queueType": "Main",
  "message": "Chat session accepted and queued."
}
```

### Accepted into overflow
```json
{
  "sessionId": "f00ba3b8-52af-43d6-89ae-7d80e0b14d9d",
  "status": "Queued",
  "queueType": "Overflow",
  "message": "Main queue full. Session accepted into overflow."
}
```

### Rejected
```json
{
  "status": "Rejected",
  "reasonCode": "QUEUE_FULL",
  "message": "Main queue is full and overflow is not available."
}
```

## 2) Register poll / heartbeat

### Endpoint
`POST /api/chat-sessions/{sessionId}/poll`

### Purpose
Register client heartbeat and return the current view of the session.

### Example response while still waiting
```json
{
  "sessionId": "4e1b3280-7d47-47c7-9a2c-a15ddfbd7cb4",
  "status": "Queued",
  "queueType": "Main"
}
```

### Example response after assignment
```json
{
  "sessionId": "4e1b3280-7d47-47c7-9a2c-a15ddfbd7cb4",
  "status": "Assigned",
  "agentId": "agent-12",
  "team": "TeamA"
}
```

## 3) Get session status

### Endpoint
`GET /api/chat-sessions/{sessionId}`

### Purpose
Return a current snapshot of the session.

## Diagnostic endpoints for demo and development

I may also expose a few lightweight diagnostic endpoints during implementation, such as:

- `GET /api/admin/queues`
- `GET /api/admin/agents`
- `GET /api/admin/teams`

These are not the business core of the task, but they are useful while demonstrating behavior.

## Simple workflow views

### Intake flow
```text
Client -> POST /chat-sessions -> Admission policy -> Main queue / Overflow / Rejected
```

### Polling flow
```text
Client -> POST /chat-sessions/{id}/poll -> Heartbeat update -> Return current status
```

### Assignment flow
```text
Worker -> Find waiting sessions -> Evaluate eligible agents -> Assign next session
```

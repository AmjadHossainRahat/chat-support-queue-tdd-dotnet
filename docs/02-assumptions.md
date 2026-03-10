# 02 - Defining Assumptions

The task is clear enough to start, but not detailed enough to remove every interpretation gap. In real delivery, this is where teams often drift without noticing it.

Rather than pretending everything is perfectly specified, I want to make the open areas visible and state the assumptions I will use during implementation.

## Working assumptions for implementation

I plan to use these assumptions:

### Office hours
- Office hours = 09:00 to 17:00
- Timezone = configurable in application settings
- Overflow allowed only inside that configured window

### Queue model
- One logical intake flow for chat sessions
- Main queue is the normal path
- Overflow is a secondary admission path when main queue is full and business rules allow it

### Shift model
- Shift windows will be configuration-driven
- Team C will be mapped to the night shift in seed/config data
- Agents whose shift has ended may finish current chats but cannot receive new chats

### Session inactivity
- Missing three expected polls marks the session inactive
- An inactive assigned session releases agent capacity
- An inactive queued session leaves the active waiting pool

### Persistence
- I will design with persistence in mind rather than pure in-memory state
- Exact storage choice can be finalized during implementation, but the model should support restart-safe behavior

## Why I am comfortable with these assumptions

None of these assumptions change the spirit of the task. They mainly turn vague operational details into explicit behavior so implementation and tests can move forward with confidence.

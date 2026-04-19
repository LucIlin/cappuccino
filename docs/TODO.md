# TODO

## Categories
### Agents
- [X] Switch to new Microsoft Agent Framework.
- [X] Implement a connection with LLM api.
- [X] Add configuration builder to Worker project.

### Infrastructure
- [X] Download Ollama locally and set up endpoint with project.

### Nice to have's (for now)
- [ ] Allow agents to reason about and pick LLMProfile from requirement.
- [ ] Create Github Action to transform YAML Authoring format to JSON wire format.

### Phase 1 - Get an agent to reason about a website.
- Goals
  - Discover the capabilities and limits of a single agent lookup on a website.
    - Can the agent consistently find the same points of interest?
    - Can the agent accomplish multistep workflows to find obscure links?
  - From the understanding gained, find test cases that will lay the groundwork for the tests the agents will
need to accomplish for a satisfactory end product.
- Areas of interest:
  - Event tickets.
  - Online products.
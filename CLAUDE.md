# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build entire solution
dotnet build Cappuccino.sln

# Run the Agents project
dotnet run --project src/Agents/Agents.csproj

# Run the Api project
dotnet run --project src/Api/Api.csproj

# Run tests (if added)
dotnet test
```

## Architecture

- API, database services and Agent orchestration will be built with dotnet technologies.
- frontend not yet determined.

The solution (`Cappuccino.sln`) has two projects:

- **`src/Agents/`** — Class library for agent orchestration. Uses `Microsoft.Agents.AI.OpenAI` (1.0.0-rc4) — the Microsoft Agent Framework with OpenAI (no Azure).
- **`src/Api/`** — A minimal ASP.NET Core Web API (currently a scaffold), intended to be the backend host.

## Pending work (TODO)

- Implement the agent pipeline in `src/Agents`.
- Connect `src/Api` to the Agents library.
- Implement LLM API connection.

## Project context

- Cappuccino(codename) will be a notification and calendar app that utilizes AI to monitor for requested events that cannot be determined in advance.
- The types of events the app will allow the user to subscribe to can be:
    * "Notify me when *a band* comes to my city"
    * "Notify me when this product goes on sale on *website*"
    * "Notify me when the northern lights are visible outside my house"
- The prompt will go through an Agent pipeline that determines feasibility, follow up questions for more context, potential locations on the web to monitor, 
determine best monitoring set up and more yet to be determined.
- Upon successful completion of an errand(working name for the hook or alert subscription) it will be added to an interface where the user can organize their errands.

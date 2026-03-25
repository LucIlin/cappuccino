# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build entire solution
dotnet build Cappuccino.sln

# Run the Api project
dotnet run --project src/Api/Api.csproj

# Run tests (if added)
dotnet test
```

## Architecture

The solution (`Cappuccino.sln`) currently has one project:

- **`src/Api/`** — A minimal ASP.NET Core Web API (currently a scaffold), intended to grow into the backend for AI agent orchestration.

## Pending work (TODO)

- Implement agent orchestration using the Microsoft Agent Framework in `src/Api` or a new dedicated project.
- Implement LLM API connection.

# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build entire solution
dotnet build Cappuccino.sln

# Run the Agents app (main UI)
dotnet run --project src/Agents/Agents.csproj

# Run the Api project
dotnet run --project src/Api/Api.csproj

# Run tests (if added)
dotnet test

# Set the required OpenAI API key (run from repo root or the Agents project dir)
dotnet user-secrets set OPENAI_KEY YOUR-API-KEY --project src/Agents
```

## Architecture

The solution (`Cappuccino.sln`) has two projects:

- **`src/Agents/`** — The main application. A Blazor Server app (Interactive Server render mode) that provides an AI chat interface over custom documents.
- **`src/Api/`** — A minimal ASP.NET Core Web API stub (currently just a weather forecast scaffold), intended to grow into the backend layer.

### Agents project — data flow

1. **Ingestion**: On first search, `SemanticSearch` triggers `DataIngestor`, which reads files from `wwwroot/Data/` using `DocumentReader`. Supported formats: `.md` (via `MarkdownReader`) and `.pdf` (via `PdfPigReader`). Files are chunked using `SemanticSimilarityChunker` (gpt-4o tokenizer) and stored in a local SQLite vector store (`vector-store.db` next to the binary).

2. **Search**: `SemanticSearch.SearchAsync` queries the SQLite vector collection (`data-agents-chunks`) using cosine distance on 1536-dimension embeddings (OpenAI `text-embedding-3-small`).

3. **Chat UI** (`Components/Pages/Chat/Chat.razor`): Uses `IChatClient` (OpenAI `gpt-4o-mini` via `GetResponsesClient`) with two AI tools registered via `AIFunctionFactory`:
   - `LoadDocuments` — triggers document ingestion
   - `Search` — queries the vector store and returns XML-formatted results

   The system prompt instructs the model to call `LoadDocuments` before any search, then emit `<citation filename='...'>` tags at the end of replies.

4. **Configuration**: The OpenAI key is read from `OPENAI_KEY` in configuration (user secrets, env var, or appsettings). Set it with `dotnet user-secrets set OPENAI_KEY <value> --project src/Agents`.

### Key types

| Type | Location | Role |
|------|----------|------|
| `DataIngestor` | `Services/Ingestion/DataIngestor.cs` | Runs the ingestion pipeline |
| `DocumentReader` | `Services/Ingestion/DocumentReader.cs` | Routes files to format-specific readers |
| `SemanticSearch` | `Services/SemanticSearch.cs` | Lazy ingestion + vector search |
| `IngestedChunk` | `Services/IngestedChunk.cs` | Vector store entity (1536-dim, cosine distance) |
| `Chat.razor` | `Components/Pages/Chat/Chat.razor` | Chat page with tool definitions and streaming |

## Pending work (TODO)

- Switch `Agents` to the Microsoft Agent Framework (currently uses raw `IChatClient` + manual tool registration).
- Implement LLM API connection in `Api` project.
- Add custom documents to `src/Agents/wwwroot/Data/` to replace the example files.

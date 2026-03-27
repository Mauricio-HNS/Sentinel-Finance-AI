# AI Copilot Architecture

Sentinel Finance AI is evolving toward an AI-native risk intelligence platform with four core layers:

1. `Responses API orchestration`
   The copilot layer is designed around the OpenAI Responses API for tool-aware, stateful reasoning.

2. `Structured outputs`
   Copilot answers are modeled as structured executive risk objects instead of raw free-form text. This keeps downstream UI predictable and evaluation-friendly.

3. `Knowledge retrieval`
   Contract playbooks, ticket summaries, and operational guidance are stored in a retrieval layer so the copilot can ground answers in evidence.

4. `Evaluation trail`
   Risk explanation quality is tracked with explicit eval scenarios and scorecards so the team can measure groundedness, actionability, and calibration over time.

## Current repository implementation

- `backend/src/Sentinel.Api/Controllers/AiController.cs`
  Exposes AI endpoints for explanations, copilot briefings, retrieval, and eval visibility.
- `backend/src/Sentinel.Infrastructure/AIInfrastructure.cs`
  Provides a local retrieval service, eval trail reader, and structured copilot fallback.
- `docs/knowledge/`
  Stores contract, ticket, and operating-model artifacts used by retrieval.
- `docs/evals/risk-copilot-evals.json`
  Stores the current eval suite definition for copilot quality tracking.

## Planned OpenAI-aligned evolution

- Replace local fallback copilot generation with a Responses API adapter.
- Use structured outputs for the executive risk schema.
- Move retrieval from local markdown search to vector-backed file search.
- Connect eval scenarios to a continuous model quality workflow.

## Official references

- Responses API: https://platform.openai.com/docs/api-reference/responses/create
- File search and tools: https://platform.openai.com/docs/guides/tools/file-search
- Retrieval: https://platform.openai.com/docs/guides/retrieval
- Evals API: https://platform.openai.com/docs/api-reference/evals
- Structured outputs announcement: https://openai.com/index/introducing-structured-outputs-in-the-api/

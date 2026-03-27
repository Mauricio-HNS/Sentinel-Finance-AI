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
  Exposes AI endpoints for explanations, copilot briefings, retrieval, AI platform status, and eval execution.
- `backend/src/Sentinel.Infrastructure/AIInfrastructure.cs`
  Provides local fallback retrieval and eval services plus OpenAI-aligned file-search and eval-run adapters.
- `backend/src/Sentinel.Infrastructure/Gateways.cs`
  Contains the OpenAI Responses API integration path for structured copilot generation and executive explanations.
- `docs/knowledge/`
  Stores contract, ticket, and operating-model artifacts used by retrieval.
- `docs/evals/risk-copilot-evals.json`
  Stores the current eval suite definition for copilot quality tracking.

## OpenAI-aligned evolution now represented in code

- Responses API path for structured copilot generation
- schema-safe explanations and risk briefings
- vector-store-ready retrieval path using `file_search`
- eval-run orchestration path for smoke-testing the risk copilot
- local fallbacks so the demo still runs without external secrets

## Official references

- Responses API: https://platform.openai.com/docs/api-reference/responses/create
- File search and tools: https://platform.openai.com/docs/guides/tools/file-search
- Retrieval: https://platform.openai.com/docs/guides/retrieval
- Evals API: https://platform.openai.com/docs/api-reference/evals
- Structured outputs announcement: https://openai.com/index/introducing-structured-outputs-in-the-api/

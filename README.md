<p align="center">
  <img src="./docs/branding/hero-banner.svg" alt="Sentinel Finance AI banner" />
</p>

# Sentinel Finance AI

<p align="center">
  <img src="https://img.shields.io/badge/Next.js-14-0f172a?style=for-the-badge&logo=next.js&logoColor=white" alt="Next.js" />
  <img src="https://img.shields.io/badge/.NET-8-0d1b2a?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/FastAPI-Prediction_Service-153b52?style=for-the-badge&logo=fastapi&logoColor=7df9c1" alt="FastAPI" />
  <img src="https://img.shields.io/badge/PostgreSQL-Ready-1e3a5f?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/OpenAI-Responses_API-0f3d2e?style=for-the-badge&logo=openai&logoColor=white" alt="OpenAI Responses API" />
  <img src="https://img.shields.io/badge/AI-Structured_Outputs-5b2333?style=for-the-badge&logo=json&logoColor=white" alt="Structured Outputs" />
</p>

**Sentinel Finance AI is a next-generation financial risk intelligence platform built to predict churn, payment delays, revenue exposure, and operational risk using predictive analytics and AI-generated explanations.**

Sentinel Finance AI is designed as a senior-level portfolio project with enterprise SaaS positioning, clean architecture, premium UI direction, and clear expansion paths for future AI and analytics capabilities.

## Why this repository feels senior

- AI-native orchestration instead of a single prompt box
- .NET orchestration layer plus isolated prediction microservice
- structured risk objects designed for auditable UI rendering
- retrieval-backed copilot briefings tied to contracts, tickets, and playbooks
- eval trail prepared to score explanation quality over time
- OpenTelemetry, Docker, Clean Architecture, and PostgreSQL-ready persistence

## Screenshots

### Dashboard

![Sentinel Finance AI Dashboard](./docs/screenshots/dashboard.png)

### Login

![Sentinel Finance AI Login](./docs/screenshots/login.png)

### Customers

![Sentinel Finance AI Customers](./docs/screenshots/customers.png)

## Product vision

Sentinel turns fragmented finance and customer-health signals into a single decision surface for revenue, risk, and operations leaders.

## MVP capabilities

- Executive dashboard with portfolio KPIs
- Customer portfolio list and customer detail view
- Composite risk scoring and category-level risk scores
- AI copilot on the customer detail experience
- OpenAI-ready structured explanation pipeline
- retrieval-backed knowledge surfacing for contracts and support tickets
- eval trail for AI quality review
- Churn and late payment prediction service
- Alert center for critical accounts
- Scenario simulator
- CSV ingestion entry point
- AI explanation endpoint for concise executive narratives

## Architecture

```text
frontend (Next.js)
    ->
backend API (ASP.NET Core 8, Clean Architecture)
    -> prediction-service (FastAPI)
    -> PostgreSQL
    -> Redis
    -> OpenAI API
```

## AI-native architecture

Sentinel is intentionally framed as a modern financial intelligence system, not just a CRUD dashboard with LLM text generation.

- `AI Copilot`: customer-level executive briefings with answer, summary, signals, actions, and follow-up questions
- `Structured Outputs`: the backend requests schema-shaped JSON so the UI can render deterministic risk objects
- `Knowledge Retrieval`: local contract, ticket, and playbook artifacts are retrieved as evidence for the copilot
- `Prediction Service`: FastAPI isolates scoring and risk heuristics so the .NET API can evolve into an orchestration layer
- `Eval Trail`: evaluation scenarios document whether the copilot behavior stays aligned as prompts and models evolve
- `Observability`: OpenTelemetry keeps the path open for tracing cross-service inference flows

## AI and platform stack

- OpenAI `Responses API`
- structured JSON schema outputs for executive risk narratives
- retrieval-style grounding over contracts, ticket intelligence, and operating playbooks
- evaluation trail ready for continuous AI quality checks
- FastAPI prediction service for decoupled risk modeling
- ASP.NET Core 8 + Clean Architecture for orchestration and domain boundaries
- PostgreSQL + EF Core persistence layer
- Redis-ready cache tier
- Next.js 14 + TypeScript + Tailwind premium frontend
- Docker Compose for local full-stack execution

## Technical decisions

- .NET API is the orchestration layer and single source of truth for frontend consumption.
- Prediction logic is isolated in FastAPI so scoring can evolve independently from the transactional API.
- Clean Architecture keeps domain rules separate from transport and infrastructure concerns.
- Demo seed services accelerate portfolio presentation while preserving a clear path to EF Core persistence.
- Copilot responses now have a real OpenAI integration path with graceful fallback behavior when no API key is configured.
- Structured AI objects keep the customer detail screen deterministic and easier to audit than free-form text blobs.
- Retrieval-backed evidence and explicit eval trails make the repository look and behave like a serious AI product surface.
- CSV imports now create persisted customers plus initial contracts, payments, usage metrics, tickets, predictions, and alerts.

## Structure

```text
sentinel-finance-ai/
  frontend/
  backend/
    src/
      Sentinel.Api/
      Sentinel.Application/
      Sentinel.Domain/
      Sentinel.Infrastructure/
    tests/
  prediction-service/
  docs/
  datasets/
  docker-compose.yml
  README.md
```

## Local run

```bash
cp .env.example .env
docker compose up --build
```

### Enable the real OpenAI path

Add your key before starting the stack:

```bash
OPENAI_API_KEY=your_key_here
OPENAI_MODEL=gpt-4.1-mini
```

When the key is present, the backend can use the OpenAI Responses API for:

- executive explanations
- structured customer copilot briefings
- schema-safe AI objects for the frontend

Without a key, Sentinel falls back to deterministic local logic so the demo still works.

### Refresh README screenshots

Start the frontend locally and run:

```bash
cd frontend
npm run capture:readme
```

Additional UI flows for customers, alerts, simulator, and upload are scaffolded in `frontend/app/`.

## Roadmap

### V2

- vector store or file-search backed retrieval instead of local markdown retrieval
- real OpenAI eval runs and prompt versioning
- Redis-backed caching
- richer auth and async recalculation jobs

### V3

- multi-tenant architecture
- MLflow and model registry
- agentic investigation workflows
- enterprise integrations

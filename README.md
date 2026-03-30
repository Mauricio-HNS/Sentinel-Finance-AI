<p align="center">
  <img src="./docs/branding/hero-banner.svg" alt="Sentinel Finance AI banner" />
</p>

# Sentinel Finance AI

<p align="center">
  <img src="https://img.shields.io/badge/Next.js-14-0f172a?style=for-the-badge&logo=next.js&logoColor=white" alt="Next.js" />
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/FastAPI-Prediction%20Service-153b52?style=for-the-badge&logo=fastapi&logoColor=7df9c1" alt="FastAPI" />
  <img src="https://img.shields.io/badge/OpenAI-Responses%20API-0f3d2e?style=for-the-badge&logo=openai&logoColor=white" alt="OpenAI" />
</p>

Sentinel Finance AI is a financial risk intelligence platform built to predict churn, payment delays, revenue exposure and operational risk through predictive analytics and AI-generated explanations.

## Why this repository stands out

- enterprise SaaS positioning instead of a generic demo dashboard
- .NET orchestration layer with isolated prediction service
- AI-ready architecture with structured outputs and retrieval-backed flows
- premium frontend presentation with dashboard, customers, alerts and AI Ops
- clear path toward multi-tenant and production-grade data infrastructure

## Product capabilities

- executive dashboard with portfolio KPIs
- customer portfolio and customer detail experience
- composite risk scoring and category-level risk breakdown
- AI copilot for executive briefings
- AI Ops control center for prompts, evals and model posture
- prediction service for churn and late-payment scoring
- CSV ingestion entry point
- alerts and simulator flows

## Screenshots

### Dashboard

![Sentinel Finance AI Dashboard](./docs/screenshots/dashboard.png)

### Login

![Sentinel Finance AI Login](./docs/screenshots/login.png)

### Customers

![Sentinel Finance AI Customers](./docs/screenshots/customers.png)

### AI Ops

![Sentinel Finance AI AI Ops](./docs/screenshots/ai-ops.png)

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

## AI stack

- OpenAI `Responses API`
- structured JSON outputs for deterministic UI rendering
- retrieval-oriented grounding over contracts, tickets and playbooks
- evaluation trail for future quality scoring
- FastAPI prediction service for isolated risk logic

## Repository structure

```text
sentinel-finance-ai/
  frontend/
  backend/
    src/
    tests/
  prediction-service/
  docs/
  datasets/
  docker-compose.yml
```

## Local run

```bash
cp .env.example .env
docker compose up --build
```

### Optional OpenAI configuration

```bash
OPENAI_API_KEY=your_key_here
OPENAI_MODEL=gpt-4.1-mini
```

Without an API key, Sentinel falls back to deterministic local behavior so the demo still works.

## Why recruiters like this repo

- clean architecture with credible AI product framing
- realistic finance and SaaS narrative
- backend, frontend and AI orchestration in one project
- polished visuals and clear expansion roadmap

## Roadmap

### Next

- richer vector-store retrieval
- real OpenAI eval runs
- Redis-backed caching
- stronger auth and async recalculation jobs

### Later

- multi-tenant architecture
- model registry integration
- agentic investigation workflows
- deeper enterprise integrations

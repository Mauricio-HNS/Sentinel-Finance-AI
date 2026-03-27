# Sentinel Finance AI Architecture

Sentinel Finance AI is structured as a modular SaaS monorepo with four primary runtime components:

- `frontend/`: Next.js executive UI focused on risk visibility, portfolio segmentation, and simulation workflows.
- `backend/`: ASP.NET Core 8 Web API following Clean Architecture boundaries.
- `prediction-service/`: FastAPI service responsible for lightweight heuristic predictions.
- `postgres` and `redis`: infrastructure dependencies for persistence and caching.

## Runtime flow

1. Analysts authenticate in the web application.
2. The Next.js frontend calls the .NET API.
3. The API aggregates customer, contract, usage, payment, and support signals.
4. The API calls the FastAPI service for predictive scoring.
5. The API optionally requests an OpenAI-generated explanation for executive narratives.
6. Aggregated risk insights are returned to the dashboard and customer detail pages.

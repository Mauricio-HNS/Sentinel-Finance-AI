from fastapi import FastAPI
from pydantic import BaseModel, Field


app = FastAPI(title="Sentinel Prediction Service", version="0.1.0")


class ScenarioRequest(BaseModel):
    customer_id: str | None = None
    days_late: int = Field(default=0, ge=0)
    usage_variation_percent: float = 0.0
    critical_tickets: int = Field(default=0, ge=0)
    contract_expiring_soon: bool = False


class PredictionResult(BaseModel):
    prediction_type: str
    score: float
    confidence: float
    model_version: str = "heuristic-v1"
    narrative: str


def clamp_score(value: float) -> float:
    return round(max(0.0, min(value, 100.0)), 1)


def compute_base_risk(payload: ScenarioRequest) -> float:
    return clamp_score(
        22
        + payload.days_late * 2.6
        + abs(min(payload.usage_variation_percent, 0)) * 0.95
        + payload.critical_tickets * 4.8
        + (8 if payload.contract_expiring_soon else 0)
    )


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.post("/predict/churn", response_model=PredictionResult)
def predict_churn(payload: ScenarioRequest) -> PredictionResult:
    score = clamp_score(18 + abs(min(payload.usage_variation_percent, 0)) * 1.1 + payload.critical_tickets * 6.0)
    return PredictionResult(
        prediction_type="churn",
        score=score,
        confidence=0.84,
        narrative="Churn risk rises with sustained usage decline and repeated critical support incidents."
    )


@app.post("/predict/late-payment", response_model=PredictionResult)
def predict_late_payment(payload: ScenarioRequest) -> PredictionResult:
    score = clamp_score(14 + payload.days_late * 3.9 + payload.critical_tickets * 1.5)
    return PredictionResult(
        prediction_type="late_payment",
        score=score,
        confidence=0.87,
        narrative="Payment delay risk is primarily explained by outstanding lateness and account stress signals."
    )


@app.post("/predict/overall-risk", response_model=PredictionResult)
def predict_overall_risk(payload: ScenarioRequest) -> PredictionResult:
    score = compute_base_risk(payload)
    return PredictionResult(
        prediction_type="overall_risk",
        score=score,
        confidence=0.82,
        narrative="Overall risk blends billing pressure, adoption change, support escalation and renewal exposure."
    )

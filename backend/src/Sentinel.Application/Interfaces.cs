namespace Sentinel.Application;

public interface ISentinelReadService
{
    ExecutiveDashboardDto GetDashboard();
    IReadOnlyList<CustomerListItemDto> GetCustomers();
    CustomerDetailDto? GetCustomer(Guid id);
    IReadOnlyList<AlertDto> GetAlerts();
    ExplanationResponse GenerateExplanation(ExplanationRequest request);
    CopilotResponseDto GetCopilotBriefing(Guid customerId, string? question = null);
    IReadOnlyList<KnowledgeChunkDto> GetKnowledgeBase(Guid customerId);
    IReadOnlyList<EvalRecordDto> GetRecentEvals();
    PredictionResponse RecalculateRisk(Guid customerId);
    PredictionResponse RunSimulation(ScenarioSimulationRequest request);
    CsvUploadResponse Upload(string fileName, Stream stream);
    AuthResponse Login(AuthRequest request);
    bool ResolveAlert(Guid alertId);
}

public interface IPredictionGateway
{
    Task<PredictionResponse> GetOverallRiskAsync(ScenarioSimulationRequest request, CancellationToken cancellationToken = default);
}

public interface IExplanationGateway
{
    Task<string> GenerateAsync(string context, CancellationToken cancellationToken = default);
}

public interface IKnowledgeRetrievalService
{
    IReadOnlyList<KnowledgeChunkDto> Retrieve(Guid customerId, string customerName, string question, int take = 4);
}

public interface IEvalsTrailService
{
    IReadOnlyList<EvalRecordDto> GetRecent();
}

public interface IAICopilotGateway
{
    Task<CopilotResponseDto> GenerateAsync(
        CustomerDetailDto detail,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge,
        CancellationToken cancellationToken = default);
}

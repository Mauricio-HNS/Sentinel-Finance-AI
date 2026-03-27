// ---Made By Destiny7 Softwares---
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
    AiPlatformStatusDto GetAiStatus();
    EvalRunResponseDto RunRiskCopilotEval();
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
    string Mode { get; }
    Task<string> GenerateAsync(string context, CancellationToken cancellationToken = default);
}

public interface IKnowledgeRetrievalService
{
    string Mode { get; }
    IReadOnlyList<KnowledgeChunkDto> Retrieve(Guid customerId, string customerName, string question, int take = 4);
}

public interface IEvalsTrailService
{
    string Mode { get; }
    IReadOnlyList<EvalRecordDto> GetRecent();
    Task<EvalRunResponseDto> RunRiskCopilotEvalAsync(CancellationToken cancellationToken = default);
}

public interface IAICopilotGateway
{
    string Mode { get; }
    Task<CopilotResponseDto> GenerateAsync(
        CustomerDetailDto detail,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge,
        CancellationToken cancellationToken = default);
}

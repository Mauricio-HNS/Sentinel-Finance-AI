namespace Sentinel.Application;

public interface ISentinelReadService
{
    ExecutiveDashboardDto GetDashboard();
    IReadOnlyList<CustomerListItemDto> GetCustomers();
    CustomerDetailDto? GetCustomer(Guid id);
    IReadOnlyList<AlertDto> GetAlerts();
    ExplanationResponse GenerateExplanation(ExplanationRequest request);
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

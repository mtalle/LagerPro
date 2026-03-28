namespace LagerPro.Contracts.Status;

public record ProjectStatusDto(
    string Project,
    string CurrentFocus,
    string TodayPriority,
    string TechnicalFocus,
    string Risks,
    string NextStep,
    string LastUpdatedUtc);

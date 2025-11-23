namespace Letters.Domain.Entities;

public class Essay
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CorrectedAt { get; set; }
    public int? Score { get; set; }
    public string? CorrectionFeedback { get; set; }
    public string? Strengths { get; set; }
    public string? Improvements { get; set; }
    public string? DetailedAnalysis { get; set; }
    
    // Relacionamento
    public User? User { get; set; }
}

namespace Letters.Application.DTOs;

public class EssayRequestDto
{
    public string Theme { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class EssayResponseDto
{
    public Guid Id { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CorrectedAt { get; set; }
    public int? Score { get; set; }
    public string? CorrectionFeedback { get; set; }
    public List<string>? Strengths { get; set; }
    public List<string>? Improvements { get; set; }
    public string? DetailedAnalysis { get; set; }
}

public class EssayCorrectionDto
{
    public int Score { get; set; }
    public string CorrectionFeedback { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
    public string DetailedAnalysis { get; set; } = string.Empty;
}

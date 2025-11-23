using Azure.Identity;
using Letters.Application.DTOs;
using Letters.Application.Interfaces;
using Letters.Domain.Entities;
using Letters.Domain.Interfaces;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel.Primitives;
using System.Text.Json;

#pragma warning disable OPENAI001

namespace Letters.Application.Services;

public class EssayService : IEssayService
{
    private readonly IEssayRepository _essayRepository;
    private readonly ChatClient _chatClient;

    public EssayService(IEssayRepository essayRepository)
    {
        _essayRepository = essayRepository;

        const string endpoint = "https://lettersopenia.openai.azure.com/openai/v1";

        BearerTokenPolicy tokenPolicy = new(
            new DefaultAzureCredential(),
            "https://cognitiveservices.azure.com/.default");

        _chatClient = new ChatClient(
            authenticationPolicy: tokenPolicy,
            model: "gpt-5-mini",
            options: new OpenAIClientOptions()
            {
                Endpoint = new Uri(endpoint),
            });
    }

    public async Task<EssayResponseDto> CreateEssayAsync(EssayRequestDto request)
    {
        var essay = new Essay
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Theme = request.Theme,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _essayRepository.CreateAsync(essay);

        return MapToDto(essay);
    }

    public async Task<EssayResponseDto> CorrectEssayAsync(Guid essayId)
    {
        var essay = await _essayRepository.GetByIdAsync(essayId);
        if (essay == null)
        {
            throw new Exception("Redação não encontrada.");
        }

        var correction = await GenerateCorrectionWithAI(essay.Theme, essay.Content);

        essay.CorrectedAt = DateTime.UtcNow;
        essay.Score = correction.Score;
        essay.CorrectionFeedback = correction.CorrectionFeedback;
        essay.Strengths = JsonSerializer.Serialize(correction.Strengths);
        essay.Improvements = JsonSerializer.Serialize(correction.Improvements);
        essay.DetailedAnalysis = correction.DetailedAnalysis;

        await _essayRepository.UpdateAsync(essay);

        return MapToDto(essay);
    }

    public async Task<EssayResponseDto> GetEssayByIdAsync(Guid essayId)
    {
        var essay = await _essayRepository.GetByIdAsync(essayId);
        if (essay == null)
        {
            throw new Exception("Redação não encontrada.");
        }

        return MapToDto(essay);
    }

    public async Task<List<EssayResponseDto>> GetUserEssaysAsync(Guid userId)
    {
        var essays = await _essayRepository.GetByUserIdAsync(userId);
        return essays.Select(MapToDto).ToList();
    }

    private async Task<EssayCorrectionDto> GenerateCorrectionWithAI(string theme, string content)
    {
        var systemPrompt = @"Você é um professor especialista em correção de redações do ENEM. 
Sua tarefa é avaliar redações seguindo os 5 critérios do ENEM:
1. Domínio da modalidade escrita formal da língua portuguesa (0-200 pontos)
2. Compreensão da proposta de redação e aplicação de conceitos (0-200 pontos)
3. Seleção, relação, organização e interpretação de informações (0-200 pontos)
4. Demonstração de conhecimento dos mecanismos linguísticos (0-200 pontos)
5. Proposta de intervenção respeitando os direitos humanos (0-200 pontos)

Retorne a avaliação em formato JSON com:
{
  ""score"": nota total (0-1000),
  ""correctionFeedback"": ""comentário geral sobre a redação"",
  ""strengths"": [""ponto forte 1"", ""ponto forte 2"", ...],
  ""improvements"": [""melhoria 1"", ""melhoria 2"", ...],
  ""detailedAnalysis"": ""análise detalhada por competência""
}";

        var userPrompt = $@"Tema: {theme}

Redação:
{content}

Avalie esta redação seguindo os critérios do ENEM e retorne apenas o JSON com a avaliação.";

        try
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            ]);

            var responseText = completion.Content[0].Text;

            // Remover possíveis markdown code blocks
            responseText = responseText.Replace("```json", "").Replace("```", "").Trim();

            var correction = JsonSerializer.Deserialize<EssayCorrectionDto>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (correction == null)
            {
                throw new Exception("Falha ao processar resposta da IA.");
            }

            return correction;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao corrigir redação com IA: {ex.Message}");
        }
    }

    private EssayResponseDto MapToDto(Essay essay)
    {
        return new EssayResponseDto
        {
            Id = essay.Id,
            Theme = essay.Theme,
            Content = essay.Content,
            CreatedAt = essay.CreatedAt,
            CorrectedAt = essay.CorrectedAt,
            Score = essay.Score,
            CorrectionFeedback = essay.CorrectionFeedback,
            Strengths = string.IsNullOrEmpty(essay.Strengths) 
                ? null 
                : JsonSerializer.Deserialize<List<string>>(essay.Strengths),
            Improvements = string.IsNullOrEmpty(essay.Improvements) 
                ? null 
                : JsonSerializer.Deserialize<List<string>>(essay.Improvements),
            DetailedAnalysis = essay.DetailedAnalysis
        };
    }
}

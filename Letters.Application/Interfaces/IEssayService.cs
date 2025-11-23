using Letters.Application.DTOs;

namespace Letters.Application.Interfaces;

public interface IEssayService
{
    Task<EssayResponseDto> CreateEssayAsync(EssayRequestDto request);
    Task<EssayResponseDto> CorrectEssayAsync(Guid essayId);
    Task<EssayResponseDto> GetEssayByIdAsync(Guid essayId);
    Task<List<EssayResponseDto>> GetUserEssaysAsync(Guid userId);
}

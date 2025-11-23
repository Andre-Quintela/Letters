using Letters.Application.DTOs;

namespace Letters.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<AuthResponseDto> Register(RegisterDto registerDto);
        Task<bool> ValidateToken(string token);
    }
}

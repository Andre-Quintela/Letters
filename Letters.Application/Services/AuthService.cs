using Letters.Application.DTOs;
using Letters.Application.Interfaces;
using Letters.Domain.Entities;
using Letters.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Letters.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _authRepository.GetByEmail(loginDto.Email);
                
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email ou senha inválidos"
                    };
                }

                var passwordHash = HashPassword(loginDto.Password);
                
                if (user.PasswordHash != passwordHash)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email ou senha inválidos"
                    };
                }

                // Gerar token simples (em produção use JWT)
                var token = GenerateToken(user);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login realizado com sucesso",
                    Token = token,
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Erro ao fazer login: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            try
            {
                if (await _authRepository.EmailExists(registerDto.Email))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email já cadastrado"
                    };
                }

                var user = new User
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    PasswordHash = HashPassword(registerDto.Password),
                    Document = registerDto.Document,
                    BornDate = registerDto.BornDate,
                    SchoolId = registerDto.SchoolId,
                    Grade = registerDto.Grade,
                    isTeacher = registerDto.IsTeacher
                };

                var createdUser = await _authRepository.Create(user);
                var token = GenerateToken(createdUser);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Cadastro realizado com sucesso",
                    Token = token,
                    User = MapToUserDto(createdUser)
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Erro ao fazer cadastro: {ex.Message}"
                };
            }
        }

        public Task<bool> ValidateToken(string token)
        {
            // Implementação simples - em produção use JWT
            return Task.FromResult(!string.IsNullOrEmpty(token));
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private string GenerateToken(User user)
        {
            // Token simples - em produção use JWT
            var tokenString = $"{user.Id}:{user.Email}:{DateTime.UtcNow.Ticks}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenString));
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Document = user.Document,
                BornDate = user.BornDate,
                SchoolId = user.SchoolId,
                Grade = user.Grade,
                isTeacher = user.isTeacher
            };
        }
    }
}

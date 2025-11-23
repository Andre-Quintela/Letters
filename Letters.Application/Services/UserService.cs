using Letters.Application.DTOs;
using Letters.Application.Interfaces;
using Letters.Domain.Entities;
using Letters.Domain.Interfaces;

namespace Letters.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public Task Add(UserDto userDto)
        {
            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                Document = userDto.Document,
                BornDate = userDto.BornDate,
                SchoolId = Guid.NewGuid(),
                Grade = userDto.Grade,
                isTeacher = false
            };

            
            return _userRepository.Add(user);
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDto>> GetAll()
        {
            var users = _userRepository.GetAll();

            // Map List<User> to List<UserDto>
            return users.ContinueWith(t => t.Result.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name
            }).ToList());
        }

        public Task<UserDto> getByEmail()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> getById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(UserDto user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.getById(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Document = user.Document ?? string.Empty,
                BornDate = user.BornDate.ToDateTime(TimeOnly.MinValue),
                SchoolId = user.SchoolId.ToString(),
                Grade = user.Grade,
                IsTeacher = user.isTeacher
            };
        }

        public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.getById(userId);
            if (user == null) return false;

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Document = dto.Document;
            user.BornDate = DateOnly.FromDateTime(dto.BornDate);
            user.SchoolId = Guid.Parse(dto.SchoolId);
            user.Grade = dto.Grade;

            await _userRepository.Update(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.getById(userId);
            if (user == null) return false;

            // Verificar senha atual - implementar verificação real depois
            // Por enquanto, apenas atualizar a senha
            user.PasswordHash = dto.NewPassword; // TODO: Hash com BCrypt
            await _userRepository.Update(user);
            return true;
        }
    }
}

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
    }
}

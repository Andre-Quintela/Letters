using Letters.Application.DTOs;

namespace Letters.Application.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto> getByEmail();
        public Task<UserDto> getById(Guid id);
        public Task Add(UserDto user);
        public Task Update(UserDto user);
        public Task Delete(Guid id);
        public Task<List<UserDto>> GetAll();
    }
}

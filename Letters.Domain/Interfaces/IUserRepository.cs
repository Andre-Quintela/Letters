using Letters.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letters.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> getByEmail();
        public Task<User> getById(Guid id);
        public Task Add(User user);
        public Task Update(User user);
        public Task Delete(Guid id);
        public Task<List<User>> GetAll();
    }
}

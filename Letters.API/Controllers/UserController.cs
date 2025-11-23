using Letters.Application.DTOs;
using Letters.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Letters.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public Task<List<UserDto>> Get()
        {
            return _userService.GetAll();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(Guid id)
        {
            var profile = await _userService.GetUserProfileAsync(id);
            
            if (profile == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(profile);
        }

        // POST api/<UserController>
        [HttpPost]
        public Task Post([FromBody] UserDto userDto)
        {
            return _userService.Add(userDto);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var success = await _userService.UpdateProfileAsync(id, dto);
            
            if (!success)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(new { message = "Perfil atualizado com sucesso" });
        }

        // POST api/<UserController>/5/change-password
        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            var success = await _userService.ChangePasswordAsync(id, dto);
            
            if (!success)
            {
                return BadRequest(new { message = "Senha atual incorreta ou usuário não encontrado" });
            }

            return Ok(new { message = "Senha alterada com sucesso" });
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

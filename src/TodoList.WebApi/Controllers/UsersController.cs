using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebApi.Models;
using TodoList.WebApi.Repositories;
using TodoList.WebApi.Services;
using TodoList.WebApi.ViewModels;

namespace TodoList.WebApi.Controllers
{
    [ApiController]
    [Route("v1/{controller}")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;

        public UsersController(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserViewModel>> Show(string username)
        {
            User user = await _userRepository.GetUserByUsername(username);
            if (user is null) return NotFound();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username
            };

            return Ok(userViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<UserViewModel>> Create(CreateUserViewModel model)
        {
            if (model.Password != model.ConfirmPassword) return BadRequest();
            
            bool userAlreadyExists = await _userRepository.Exists(model.Username);
            if (userAlreadyExists) return Conflict();

            string hashPassword = _passwordHasher.Hash(model.Password);
            var user = new User
            {
                Username = model.Username
            };
            await _userRepository.Insert(user, hashPassword);

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username
            };

            return CreatedAtAction(nameof(Show), new { username = model.Username }, userViewModel);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseViewModel>> Authenticate(LoginViewModel model)
        {
            User user = await _userRepository.GetUserWithPasswordByUsername(model.Username);

            if (user is null) return NotFound();
            if (!_passwordHasher.VerifyPassword(user.Password, model.Password)) return BadRequest();

            string token = _tokenService.GenerateToken(user);

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username
            };

            var loginResponseViewModel = new LoginResponseViewModel
            {
                Token = token,
                Data = userViewModel
            };

            return Ok(loginResponseViewModel);
        }
    }
}

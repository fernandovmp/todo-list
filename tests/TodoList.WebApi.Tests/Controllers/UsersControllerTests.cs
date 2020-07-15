using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.WebApi.Controllers;
using TodoList.WebApi.Models;
using TodoList.WebApi.Repositories;
using TodoList.WebApi.Services;
using TodoList.WebApi.ViewModels;
using Xunit;

namespace TodoList.WebApi.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task Create_UnconfirmedPassword_ReturnsBadRequest()
        {
            var model = new CreateUserViewModel
            {
                Username = "default",
                Password = "password",
                ConfirmPassword = "diferentPassword"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<UserViewModel> result = await usersController.Create(model);

            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Create_WithExistingUsername_ReturnsConflict()
        {
            var model = new CreateUserViewModel
            {
                Username = "default",
                Password = "password",
                ConfirmPassword = "password"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<UserViewModel> result = await usersController.Create(model);

            result.Result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async Task Create_ValidUser_ReturnsCreated()
        {
            var model = new CreateUserViewModel
            {
                Username = "default",
                Password = "password",
                ConfirmPassword = "password"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.Exists(It.IsAny<string>()))
                .ReturnsAsync(false);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            fakePasswordHasher
                .Setup(passwordHasher => passwordHasher.Hash(It.IsAny<string>()))
                .Returns("HashedPassword");
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<UserViewModel> result = await usersController.Create(model);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Create_ValidUser_ReturnsUserWithId()
        {
            int expectedId = 1;
            var model = new CreateUserViewModel
            {
                Username = "default",
                Password = "password",
                ConfirmPassword = "password"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.Exists(It.IsAny<string>()))
                .ReturnsAsync(false);
            fakeUserRepository
                .Setup(repository => repository.Insert(It.IsAny<User>(), It.IsAny<string>()))
                .Callback((User user, string password) => user.Id = expectedId)
                .Returns(Task.CompletedTask);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            fakePasswordHasher
                .Setup(passwordHasher => passwordHasher.Hash(It.IsAny<string>()))
                .Returns("HashedPassword");
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<UserViewModel> result = await usersController.Create(model);

            result
                .Result
                .As<CreatedAtActionResult>()
                .Value
                .As<UserViewModel>()
                .Id
                .Should()
                .Be(expectedId);
        }

        [Fact]
        public async Task Authenticate_NonexistentUser_ReturnsNotFound()
        {
            var model = new LoginViewModel
            {
                Username = "default",
                Password = "password"
            };

            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.GetUserByUsername(It.IsAny<string>()))
                .Returns(Task.FromResult<User>(null));
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<LoginResponseViewModel> result = await usersController.Authenticate(model);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Authenticate_WrongPassword_ReturnsBadRequest()
        {
            var model = new LoginViewModel
            {
                Username = "default",
                Password = "password"
            };
            var userStoredInDatabase = new User
            {
                Username = "default",
                Password = "diferentPassword"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.GetUserWithPasswordByUsername(It.IsAny<string>()))
                .ReturnsAsync(userStoredInDatabase);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            fakePasswordHasher
                .Setup(passwordHasher => passwordHasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<LoginResponseViewModel> result = await usersController.Authenticate(model);

            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsOk()
        {
            var model = new LoginViewModel
            {
                Username = "default",
                Password = "password"
            };
            var userStoredInDatabase = new User
            {
                Username = "default",
                Password = "password"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.GetUserWithPasswordByUsername(It.IsAny<string>()))
                .ReturnsAsync(userStoredInDatabase);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            fakePasswordHasher
                .Setup(passwordHasher => passwordHasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<LoginResponseViewModel> result = await usersController.Authenticate(model);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsLoginResponseViewModel()
        {
            var model = new LoginViewModel
            {
                Username = "default",
                Password = "password"
            };
            var userStoredInDatabase = new User
            {
                Username = "default",
                Password = "password"
            };
            var fakeUserRepository = new Mock<IUserRepository>();
            fakeUserRepository
                .Setup(repository => repository.GetUserWithPasswordByUsername(It.IsAny<string>()))
                .ReturnsAsync(userStoredInDatabase);
            var fakePasswordHasher = new Mock<IPasswordHasherService>();
            fakePasswordHasher
                .Setup(passwordHasher => passwordHasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var fakeTokenService = new Mock<ITokenService>();
            var usersController = new UsersController(
                fakeUserRepository.Object,
                fakePasswordHasher.Object,
                fakeTokenService.Object);

            ActionResult<LoginResponseViewModel> result = await usersController.Authenticate(model);

            result
                .Result
                .As<OkObjectResult>()
                .Value
                .Should()
                .BeOfType<LoginResponseViewModel>();
        }
    }
}

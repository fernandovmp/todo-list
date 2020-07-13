using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.WebApi.Controllers;
using TodoList.WebApi.Models;
using TodoList.WebApi.Repositories;
using Xunit;

namespace TodoList.WebApi.Tests.Controllers
{
    public class TodoItemsControllerTests
    {
        [Fact]
        public async Task Create_ValidTodoItem_ReturnsTodoItemWithId()
        {
            int expectedId = 1;
            var model = GetValidTodoItem();

            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.Insert(It.IsAny<TodoItem>()))
                .Callback<TodoItem>(todo => todo.Id = expectedId)
                .Returns(Task.CompletedTask);

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            ActionResult<TodoItem> result = await todoItemsController.Create(model);

            result
                .Result
                .As<CreatedAtActionResult>()
                .Value
                .As<TodoItem>()
                .Id
                .Should()
                .Be(expectedId);
        }

        [Fact]
        public async Task Create_ValidTodoItem_ReturnsTodoItemWithUserId()
        {
            var model = GetValidTodoItem();
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.Insert(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");
            int expectedUserId = 1;

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Create(model);

            result
                .Result
                .As<CreatedAtActionResult>()
                .Value
                .As<TodoItem>()
                .UserId
                .Should()
                .Be(expectedUserId);
        }

        [Fact]
        public async Task Create_ValidTodoItem_ReturnsCreatedResult()
        {
            var model = GetValidTodoItem();
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.Insert(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Create(model);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Update_NonexistentId_ReturnsNotFound()
        {
            int id = 1;
            var model = GetValidTodoItem();
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult<TodoItem>(null));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Update(id, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_OtherUserTodoItem_ReturnsForbid()
        {
            int id = 1;
            var model = GetValidTodoItem();
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 2);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Update(id, model);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Update_ExistingTodoItem_ReturnsNoContent()
        {
            int id = 1;
            var model = GetValidTodoItem();
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 1);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Update(id, model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_NonexistentId_ReturnsNotFound()
        {
            int id = 1;
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult<TodoItem>(null));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Delete(id);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_OtherUserTodoItem_ReturnsForbid()
        {
            int id = 1;
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 2);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Delete(id);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Delete_ExistingTodoItem_ReturnsNoContent()
        {
            int id = 1;
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 1);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult result = await todoItemsController.Delete(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Show_NonexistentId_ReturnsNotFound()
        {
            int id = 1;
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult<TodoItem>(null));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Show(id);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Show_OtherUserTodoItem_ReturnsForbid()
        {
            int id = 1;
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 2);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Show(id);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Show_ExistingTodoItem_ReturnsNoContent()
        {
            int id = 1;
            var todoItemStoredInDatabase = GetValidTodoItemWithUserId(userId: 1);
            var fakeTodoRepository = new Mock<ITodoItemRepository>();
            fakeTodoRepository
                .Setup(repository => repository.GetTodoById(It.IsAny<int>()))
                .Returns(Task.FromResult(todoItemStoredInDatabase));

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser(identityName: "1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Show(id);

            result
                .Result
                .As<OkObjectResult>()
                .Value
                .As<TodoItem>()
                .Should()
                .BeSameAs(todoItemStoredInDatabase);
        }

        private TodoItem GetValidTodoItemWithUserId(int userId)
        {
            return new TodoItem
            {
                Id = 1,
                Title = "Todo Task",
                Completed = false,
                UserId = userId
            };
        }

        private TodoItem GetValidTodoItem()
        {
            return new TodoItem
            {
                Title = "Todo Task",
                Completed = false
            };
        }

        private ControllerContext GetFakeControlerContextWithFakeUser(string identityName)
        {
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext
                .Setup(httpContext => httpContext.User.Identity.Name)
                .Returns(identityName);
            var fakeControllerContext = new ControllerContext
            {
                HttpContext = fakeHttpContext.Object
            };
            return fakeControllerContext;
        }

    }
}

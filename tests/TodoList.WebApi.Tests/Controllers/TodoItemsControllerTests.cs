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

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser("1");
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

            ControllerContext fakeControllerContext = GetFakeControlerContextWithFakeUser("1");

            var todoItemsController = new TodoItemsController(fakeTodoRepository.Object);
            todoItemsController.ControllerContext = fakeControllerContext;

            ActionResult<TodoItem> result = await todoItemsController.Create(model);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
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

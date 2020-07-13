using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebApi.Models;
using TodoList.WebApi.Repositories;

namespace TodoList.WebApi.Controllers
{
    [ApiController]
    [Route("v1/{controller}")]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemRepository _todoItemRepository;

        public TodoItemsController(ITodoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Index()
        {
            int userId = int.Parse(User.Identity.Name);
            IEnumerable<TodoItem> todoItems = await _todoItemRepository.GetTodoItems(userId);
            return Ok(todoItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Show(int id)
        {
            TodoItem todoItem = await _todoItemRepository.GetTodoById(id);
            if (todoItem is null) return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (todoItem.UserId != userId) return Forbid();

            return Ok(todoItem);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            int userId = int.Parse(User.Identity.Name);
            todoItem.UserId = userId;
            await _todoItemRepository.Insert(todoItem);
            return CreatedAtAction(nameof(Show), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TodoItem model)
        {
            TodoItem todoItem = await _todoItemRepository.GetTodoById(id);
            if (todoItem is null) return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (todoItem.UserId != userId) return Forbid();

            await _todoItemRepository.Update(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            TodoItem todoItem = await _todoItemRepository.GetTodoById(id);
            if (todoItem is null) return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (todoItem.UserId != userId) return Forbid();

            await _todoItemRepository.Delete(id);
            return NoContent();
        }
    }
}

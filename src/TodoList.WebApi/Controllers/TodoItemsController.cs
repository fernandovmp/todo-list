using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebApi.Repositories;

namespace TodoList.WebApi.Controllers
{
    [ApiController]
    [Route("v1/{controller}")]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemRepository _todoItemRepository;

        public TodoItemsController(ITodoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IEnumerable<TodoItem> todoItems = await _todoItemRepository.GetTodoItems();
            return Ok(todoItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Show(int id)
        {
            TodoItem todoItem = await _todoItemRepository.GetTodoById(id);
            if (todoItem is null) return NotFound();
            return Ok(todoItem);
        }

        [HttpPost]
        public async Task<ActionResult> Create(TodoItem todoItem)
        {
            await _todoItemRepository.Insert(todoItem);
            return CreatedAtAction(nameof(Show), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TodoItem todoItem)
        {
            await _todoItemRepository.Update(id, todoItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _todoItemRepository.Delete(id);
            return NoContent();
        }
    }
}

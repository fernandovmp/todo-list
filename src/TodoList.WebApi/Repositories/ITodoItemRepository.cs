using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.WebApi.Models;

namespace TodoList.WebApi.Repositories
{
    public interface ITodoItemRepository
    {
        Task<IEnumerable<TodoItem>> GetTodoItems(int userId);
        Task<TodoItem> GetTodoById(int id);
        Task Insert(TodoItem todoItem);
        Task Update(int id, TodoItem todoItem);
        Task Delete(int id);
        Task<bool> Exists(int id);
    }
}

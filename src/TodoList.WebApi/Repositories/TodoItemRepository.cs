using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using TodoList.WebApi.Models;

namespace TodoList.WebApi.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly IDbConnection _connection;

        public TodoItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<TodoItem>> GetTodoItems(int userId)
        {
            string query = @"select id, title, completed from TodoItems where userId = @UserId;";
            IEnumerable<TodoItem> todoItems = await _connection.QueryAsync<TodoItem>(query, new
            {
                UserId = userId
            });
            return todoItems;
        }
        public async Task<TodoItem> GetTodoById(int id)
        {
            string query = @"select id, title, completed, userId from TodoItems where id = @Id;";
            TodoItem todoItem = await _connection.QuerySingleOrDefaultAsync<TodoItem>(query, new
            {
                Id = id
            });
            return todoItem;
        }
        public async Task Insert(TodoItem todoItem)
        {
            string query = @"insert into TodoItems (title, completed, userId) output inserted.id 
                             values (@Title, @Completed, @UserId);";
            int todoItemId = await _connection.ExecuteScalarAsync<int>(query, todoItem);
            todoItem.Id = todoItemId;
        }
        public async Task Update(int id, TodoItem todoItem)
        {
            string query = @"update TodoItems
                            set title = @Title,
                            completed = @Completed 
                            where id = @Id;";
            await _connection.ExecuteAsync(query, new
            {
                Id = id,
                todoItem.Title,
                todoItem.Completed
            });
        }
        public async Task Delete(int id)
        {
            string query = @"delete from TodoItems where id = @Id;";
            await _connection.ExecuteAsync(query, new
            {
                Id = id
            });
        }

        public async Task<bool> Exists(int id)
        {
            string query = @"select 1 from TodoItems where id = @Id;";
            bool result = await _connection.ExecuteScalarAsync<bool>(query, new { id = id });
            return result;
        }
    }
}

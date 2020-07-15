using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using TodoList.WebApi.Models;

namespace TodoList.WebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<bool> Exists(string username)
        {
            string query = @"select 1 from Users where username = @Username;";
            bool result = await _connection.ExecuteScalarAsync<bool>(query, new { Username = username });
            return result;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            string query = @"select id, username from Users where username = @Username;";
            User user = await _connection.QuerySingleOrDefaultAsync<User>(query, new
            {
                Username = username,
            });
            return user;
        }

        public async Task<User> GetUserWithPasswordByUsername(string username)
        {
            string query = @"select id, password, username from Users where username = @Username;";
            User user = await _connection.QuerySingleOrDefaultAsync<User>(query, new
            {
                Username = username,
            });
            return user;
        }

        public async Task Insert(User user, string password)
        {
            string query = @"insert into Users (username, password) output inserted.id values (@Username, @Password);";
            int userId = await _connection.ExecuteScalarAsync<int>(query, new
            {
                user.Username,
                Password = password
            });
            user.Id = userId;
        }
    }
}

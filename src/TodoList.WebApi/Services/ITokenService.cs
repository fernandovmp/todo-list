using TodoList.WebApi.Models;

namespace TodoList.WebApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}

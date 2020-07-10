namespace TodoList.WebApi.Services
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool VerifyPassword(string hash, string password);
    }
}

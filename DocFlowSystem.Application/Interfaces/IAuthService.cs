namespace DocFlowSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string email, string password, string role);
        Task<bool> LoginAsync(string email, string password);
    }
}
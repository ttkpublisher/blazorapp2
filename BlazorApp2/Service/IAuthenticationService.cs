namespace BlazorApp2.Service
{
    public interface IAuthenticationService
    {
        Task<string> SignInAsync(string username, string password);
    }
}

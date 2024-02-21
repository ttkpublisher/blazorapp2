using IdentityModel.Jwk;

namespace BlazorApp2.Service
{
    public interface IAuthenticationService
    {
        Task<string> RefreshToken(string idToken);
        Task<string> SignInAsync(string username, string password);
        Task<string> GetIdToken(string code);
        Task<bool> CheckToken(string idToken);
    }
}

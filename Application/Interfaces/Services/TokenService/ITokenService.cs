

namespace Application.Interfaces.Services.TokenService
{
    public interface ITokenService
    {
        string CreateToken(string userId, string email, string fullName, IList<string> roles);
    }
}

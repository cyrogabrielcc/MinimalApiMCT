using MinimalApi.models;

namespace MinimalApi.Services
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}

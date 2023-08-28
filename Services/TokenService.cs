using Microsoft.IdentityModel.Tokens;
using MinimalApi.models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApi.Services
{
    public class TokenService : ITokenService
    {
        string ITokenService.GerarToken(string key, string issuer, string audience, UserModel user)
        {
            // Declarações sobre o user que vão compor o token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            // codificando rede de bytes
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // dizendo a codificação 
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer:issuer,
                                        audience: audience,
                                        claims:claims,
                                        expires: DateTime.Now.AddMinutes(120) ,
                                        signingCredentials: credentials);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;


        }
    }
}

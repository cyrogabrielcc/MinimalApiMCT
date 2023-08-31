using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MinimalApi.models;
using MinimalApi.Services;

namespace MinimalApi.ApiEndpoints
{
    public static class AutenticacaoEndpoints
    {
        public static void MapAuthenticacaoEndpoints(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null)
                {
                    return Results.BadRequest();
                }

                if (userModel.UserName == "Cyro" && userModel.Password == "Xyz@1230")
                {
                    var tokenString = tokenService.GerarToken(
                            app.Configuration["Jwt:Key"],
                            app.Configuration["Jwt:Issuer"],
                            app.Configuration["Jwt:Audience"],
                            userModel);
                    return Results.Ok(new { token = tokenString });
                }

                else
                {
                    return Results.BadRequest("Login inválido");
                }

            }).Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status200OK)
            .WithName("Login")
            .WithTags("Autenticacao");
        }
    }
}
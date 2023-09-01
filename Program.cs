using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Context;
using MinimalApi.models;
using MinimalApi.Services;
using MinimalApi.ApiEndpoints;
using MinimalApi.AppServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.
    Services.
    AddDbContext<AppDbContext>(
        options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("ConexaoPadrao")
            )
        );

//definir os endpoints
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,

               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidAudience = builder.Configuration["Jwt:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

           };
       });

builder.Services.AddAuthorization();

var app = builder.Build();


app.MapAuthenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutossEndpoints();
var environment = app.Environment;
app.UseExceptionHandling(environment).UseSwaggerMiddlare().UseAppCors();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
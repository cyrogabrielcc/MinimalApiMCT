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

builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJwt();

var app = builder.Build();


app.MapAuthenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutossEndpoints();
var environment = app.Environment;
app.UseExceptionHandling(environment).UseSwaggerMiddlare().UseAppCors();
app.UseAuthentication();
app.UseAuthorization();



app.Run();
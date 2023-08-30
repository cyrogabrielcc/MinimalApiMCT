using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Context;
using MinimalApi.models;
using MinimalApi.Services;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apiagenda", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n Enter 'Bearer'[space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

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


// ------------- ENDPOINTS -----
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
  .WithTags("Autenticacao") ;


// Criando o método Post
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();
    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
}).RequireAuthorization();

// Get pra trazer todos os valores
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

// Get retornando um id
app.MapGet("/categorias/{id}/", async (int id, AppDbContext db) =>
{
    return await db.Categorias.FindAsync(id) is Categoria categoria ?
            Results.Ok(categoria) : Results.NotFound();
}).RequireAuthorization();

// Ataulizando com o método PUT
app.MapPut("/categorias/{id}/", async (int id, Categoria categoria, AppDbContext db) =>
{
    // busca se  o ID tá lá msm 
    if (categoria.CategoriaId != id) return Results.BadRequest();
    // Retorna os dados existentes
    var categoriaDB = await db.Categorias.FindAsync(id);
    // Verifica se o é falso
    if (categoriaDB is null) return Results.NotFound();
    // Alterações
    categoriaDB.Nome = categoria.Nome;
    categoriaDB.Descricao = categoria.Descricao;
    // Salvando e retornando o objeto
    await db.SaveChangesAsync();
    return Results.Ok(categoria);
}).RequireAuthorization();


// Criando o método Delete
app.MapDelete("/categorias/{id}/", async (int id, AppDbContext db) =>
{
    // procura a categoria
    var categoria = await db.Categorias.FindAsync(id);
    // Vendo se a categoria existe
    if (categoria is null) return Results.NotFound();
    // Removendo ategoria
    db.Categorias.Remove(categoria);
    // Salvando
    await db.SaveChangesAsync();
    // retornando
    return Results.NoContent();
}).RequireAuthorization();




/* ------------ CRIANDO OS ENDPOINTS PARA PRODUTO ------------*/



// Criando o primeiro produto
app.MapPost("/produtos/", async (Produto produto, AppDbContext db) =>
{
    // Adicionando o produto
    db.Produtos.Add(produto);
    // Salvando o produto
    await db.SaveChangesAsync();
    // Retorna Ok
    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
}).RequireAuthorization();

// Retornando todos os produtos
app.MapGet("/produtos/", async (AppDbContext db) =>
{
    await db.Produtos.ToArrayAsync();
}).RequireAuthorization();

//Retornando um único produto
app.MapGet("/produtos/{id}", async (int id, AppDbContext db) =>
{
    // Procurando o produto e vendo se é um objeto produto
    return await db.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound();
}).RequireAuthorization();

app.MapPut("/produtos/{id}", async (int id, Produto produto, AppDbContext db) =>
{

    if (produto.ProdutoId != id)
    {
        return Results.BadRequest();
    }
    // buscando o produto
    var produtodb = await db.Produtos.FindAsync(id);
    // Vendo se o produto é nulo
    if (produtodb is null) { return Results.BadRequest(); }
    // Fazendo alterações
    produtodb.Nome = produto.Nome;
    produtodb.Descricao = produto.Descricao;
    produtodb.Preco = produto.Preco;
    produtodb.DatacCompra = produto.DatacCompra;
    produtodb.Estoque = produto.Estoque;
    produtodb.CategoriaId = produto.CategoriaId;
    // Salvando
    await db.SaveChangesAsync();
    // Retornando
    return Results.Ok(produtodb);
});

app.MapDelete("/produtos/{id}", async (int id, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);

    if (produto is null) return Results.BadRequest();

    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();

    return Results.NoContent();

});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.Run();
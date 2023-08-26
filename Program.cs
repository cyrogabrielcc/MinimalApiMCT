using Microsoft.EntityFrameworkCore;
using MinimalApi.Context;
using MinimalApi.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Criando o método Get
app.MapGet("/", () => "Catálogo de produtos - 2022");

// Criando o método Post
app.MapPost("/Categorias", async (Categoria categoria, AppDbContext db) => 
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// Get pra trazer todos os valores
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToArrayAsync()); 

// Get retornando um id
app.MapGet("/categorias/{id:int}",  async (int id, AppDbContext db) => {
    return await db.Categorias.FindAsync(id) is Categoria categoria ? 
            Results.Ok(categoria) : Results.NotFound();
});

// Ataulizando com o método PUT
app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db)=>{

    // busca se  o ID tá lá msm 
    if (categoria.CategoriaId == id ) return Results.BadRequest();
    
    // Retorna os dados existentes
    var categoriaDB = await db.Categorias.FindAsync(id);

    // Verifica se o é falso
    if (categoriaDB is null) return Results.NotFound();

    // Alterações
    categoriaDB.Nome =categoria.Nome;
    categoriaDB.Descricao =categoria.Descricao;
    
    // Salvando e retornando o objeto
    await db.SaveChangesAsync();
    return Results.Ok(categoria);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   
}



app.Run();
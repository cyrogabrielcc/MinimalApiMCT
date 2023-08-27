using Microsoft.AspNetCore.Http.HttpResults;
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
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) => 
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// Get pra trazer todos os valores
app.MapGet("/categorias", async (AppDbContext db) =>
    await db.Categorias.ToArrayAsync()
 ); 

// Get retornando um id
app.MapGet("/categorias/{id:int}",  async (int id, AppDbContext db) => {
    return await db.Categorias.FindAsync(id) is Categoria categoria ? 
            Results.Ok(categoria) : Results.NotFound();
});

// Ataulizando com o método PUT
app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db)=>{
    // busca se  o ID tá lá msm 
    if (categoria.CategoriaId != id ) return Results.BadRequest();
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


// Criando o método Delete
app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db)=>{

    // procura a categoria
    var categoria = await db.Categorias.FindAsync(id);

    // Vendo se a categoria existe
    if(categoria is null) return Results.NotFound();

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();

    return Results.NoContent();
});
/* ------------ CRIANDO OS ENDPOINTS PARA PRODUTO ------------*/

// Criando o primeiro produto
app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>{
    // Adicionando o produto
    db.Produtos.Add(produto);
    // Salvando o produto
    await db.SaveChangesAsync();
    // Retorna Ok
    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
});

// Retornando todos os produtos
app.MapGet("/produtos", async (int id, AppDbContext db)=>{
    await db.Produtos.ToArrayAsync();
});

//Retornando um único produto
app.MapGet("/produtos/{int:id}", async (int id, AppDbContext db)=>{
    // Procurando o produto e vendo se é um objeto produto
    return await db.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound(); 
});

app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) => {
    // vendo se o id ta la msm
    if(produto.ProdutoId != id) return Results.BadRequest();
    // buscando o produto
    var produtodb = await db.Produtos.FindAsync(id);
    // Vendo se o produto é nulo
    if (produtodb is null) {return Results.BadRequest();}
    // Fazendo alterações
    produtodb.Nome = produto.Nome;
    produtodb.Descricao = produto.Descricao;
    produtodb.Preco = produto.Preco;
    produtodb.DatacCompra = produto.DatacCompra;
    produtodb.Estoque = produto.Estoque;
    // Salvando
    await db.SaveChangesAsync();
    // Retornando
    return Results.Ok(produto);
});

app.MapDelete("/produtos/{int:id}", async (int id, AppDbContext db)=> {
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



app.Run();
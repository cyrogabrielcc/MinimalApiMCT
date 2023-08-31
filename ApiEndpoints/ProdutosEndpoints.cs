using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Context;
using MinimalApi.models;

namespace MinimalApi.ApiEndpoints
{
    public static class ProdutosEndpoints
    {
        public static void MapProdutossEndpoints(this WebApplication app)
        {
           
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


        }
    }
}
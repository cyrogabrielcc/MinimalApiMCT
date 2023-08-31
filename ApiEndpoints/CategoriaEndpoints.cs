using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Context;
using MinimalApi.models;

namespace MinimalApi.ApiEndpoints
{
    public static class CategoriaEndpoints
    {
        public static void MapCategoriasEndpoints(this WebApplication app)
        {
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

        }
    }
}
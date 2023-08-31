using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace MinimalApi.AppServiceExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, 
                                                                    IWebHostEnvironment environment)
        {
                if (environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                
                return app;
        }
        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(p=>{
                p.AllowAnyOrigin();
                p.WithMethods("GET");
                p.AllowAnyHeader();
            });

            return app;
        }
    }
}
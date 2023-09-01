using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApi.Context;
using MinimalApi.Services;

namespace MinimalApi.AppServiceExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AppApiSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwagger();
            return builder;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
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
            return services;
        }

        public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.
                Services.
                AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(
                        builder.Configuration.GetConnectionString("ConexaoPadrao")
                        )
                    );

            builder.Services.AddSingleton<ITokenService>(new TokenService());
        }
    }
}
using ApiRest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace ApiRest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection ServiceAddIdentityServer(this IServiceCollection services)
        public static IServiceCollection AddOauthSecurity(this IServiceCollection services)
        {
            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.RequireHttpsMetadata = false;
            //        options.Authority = ApiIdentityServerConfig.Authority;
            //        options.Audience = ApiIdentityServerConfig.ClientId;
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidTypes = new[] { "at+jwt" }
            //        };
            //    })
            //    .AddOAuth2Introspection("introspection", options =>
            //    {
            //        options.Authority = ApiIdentityServerConfig.Authority;
            //        options.ClientId = ApiIdentityServerConfig.ClientId;
            //        options.ClientSecret = ApiIdentityServerConfig.Secret;
            //    });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", ApiIdentityServerConfig.RequiredScopes);
            //    });
            //});
            return services;
        }

        public static IServiceCollection AddHttpClientConfig(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddHttpClient("api", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            });

            //services.AddHttpClient("ZendeskApi", client =>
            //{
            //    client.BaseAddress = new Uri(ZendeskCredentialConfig.URLBase);
            //    client.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Basic",
            //            Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
            //            $"{ZendeskCredentialConfig.User}:{ZendeskCredentialConfig.Password}")));

            //    client.DefaultRequestHeaders.Accept
            //    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //});
            return services;
        }
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider()
                   .GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    c.UseInlineDefinitionsForEnums();
                }
                //c.AddSecurityDefinition("Bearer",
                //    new OpenApiSecurityScheme
                //    {
                //        Name = "Authorization",
                //        Type = SecuritySchemeType.Http,
                //        Scheme = "Bearer",
                //        BearerFormat = "JWT",
                //        In = ParameterLocation.Header,
                //        Description = "JWT Authorization header using the Bearer scheme."
                //    });
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                //        {
                //                new OpenApiSecurityScheme
                //                {
                //                    Reference = new OpenApiReference
                //                    {
                //                        Type = ReferenceType.SecurityScheme,
                //                        Id = "Bearer"
                //                    }
                //                },
                //                new string[] {}
                //            }
                //        });
                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });
            return services;
        }

        public static IServiceCollection AddInjection(this IServiceCollection services)
        {
            //services
            //services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IArticleService, ArticleService>();

            return services;
        }
        public static IServiceCollection AddVersionIng(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
            services.AddVersionedApiExplorer(
              options =>
              {
                  options.GroupNameFormat = "'v'VVV";
                  options.SubstituteApiVersionInUrl = true;
              });
            return services;
        }

        #region Private
        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Service",
                Version = description.ApiVersion.ToString(),
                Description = "Service API",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private class RemoveVersionFromParameter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var versionParameter = operation.Parameters.Single(p => p.Name == "x-api-version");
                operation.Parameters.Remove(versionParameter);
                versionParameter = operation.Parameters.Single(p => p.Name == "version");
                operation.Parameters.Remove(versionParameter);
            }
        }
        private class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var paths = new OpenApiPaths();
                foreach (var path in swaggerDoc.Paths)
                {
                    paths.Add(path.Key.Replace("{version}", swaggerDoc.Info.Version), path.Value);
                }
                swaggerDoc.Paths = paths;
            }
        }
        #endregion
    }
}

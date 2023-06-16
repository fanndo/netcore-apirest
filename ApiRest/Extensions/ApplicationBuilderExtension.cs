using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ApiRest.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder AddConfigure(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllers().RequireAuthorization();
            });

            return app;
        }
        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
             options =>
             {
                 // build a swagger endpoint for each discovered API version  
                 foreach (var description in provider.ApiVersionDescriptions)
                 {
                     options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                 }
             });

            return app;
        }
        public static IApplicationBuilder AddHealthChecks(this IApplicationBuilder app)
        {
            //app.UseHealthChecks("/healthcheck", new HealthCheckOptions()
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            //app.UseHealthChecks("/ready", new HealthCheckOptions
            //{
            //    Predicate = registration => registration.Tags.Contains("dependencies"),
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            return app;
        }

        public class LogContentTypeProvider : Microsoft.AspNetCore.StaticFiles.IContentTypeProvider
        {
            public bool TryGetContentType(string subpath, out string contentType)
            {
                contentType = "text/plain";
                return true;
            }
        }
    }
}

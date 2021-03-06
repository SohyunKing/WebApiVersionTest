using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiVersionByHeader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(c =>
            c.Conventions.Add(
            new ApiExplorerGroupPerVersionConvention()) // decorate Controllers to distinguish SwaggerDoc (v1, v2, etc.)
            );
            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.Conventions.Add(new VersionByNamespaceConvention());
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                //options.ApiVersionSelector = new FallBackApiVersionSelector(options);
                ////header
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //query string
                //options.ApiVersionReader = new QueryStringApiVersionReader("v");
                //route
                //options.ApiVersionReader = new UrlSegmentApiVersionReader();
                //combine
                //options.ApiVersionReader = ApiVersionReader.Combine(
                //    new QueryStringApiVersionReader("v"),
                //    new HeaderApiVersionReader("api-version"));
            });

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Swagger Document",
                    Description = "A Web API",
                    TermsOfService = new Uri("https://example.com/terms"),

                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.OperationFilter<SwaggerParameterAttributeFilter>();
                //c.ResolveConflictingActions(descriptions =>
                //{

                //    var versionDes = descriptions.FirstOrDefault(des => des.GroupName == "v1_0");
                //    return versionDes;
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger V1");
                //c.RoutePrefix = string.Empty;
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.v1"
                var apiVersion = controllerNamespace?.Split('.').Last().ToLower();
                if (apiVersion != null && apiVersion.Contains("_"))
                {
                    var split = apiVersion.Split('_');
                    apiVersion = split[1] == "0" ? split[0] : apiVersion.Replace('_', '.');
                }

                controller.ApiExplorer.GroupName = apiVersion;
                //controller.ApiExplorer.GroupName = "";
            }
        }
    }
}

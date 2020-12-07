using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using PublicElections.Api.Extensions;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Ioc;
using PublicElections.Infrastructure.Options;

namespace PublicElections.Api
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddOptions<JwtSettings>().Bind(Configuration.GetSection("JwtSettings"));

            var sqlSettings = Configuration.GetSection("SqlSettings").Get<SqlSettings>();
            services.AddOptions<SqlSettings>().Bind(Configuration.GetSection("SqlSettings"));

            services.AddOptions<MailSettings>().Bind(Configuration.GetSection("MailSettings"));
            services.AddOptions<WebSettings>().Bind(Configuration.GetSection("WebSettings"));

            services.AddJWT(jwtSettings);
            services.AddSwagger();
            services.AddSqlDb(sqlSettings);

            services.AddServices();

            services.AddSignalR();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                                  });
            });

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);

            IdentityModelEventSource.ShowPII = true;

            var swaggerOptions = Configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();

            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOptions.JsonRoute;
            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoNetCore.Controllers;
using ToDoNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace ToDoNetCore
{
    public class Startup
    {
        public AppSettings AppSettings { get; set; }
        public IConfiguration Configuration { get; }
        private const string DefaultCorsPolicyName = "localhost";

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = (new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()).Build();
            AppSettings = configuration.Get<AppSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName)));
            services.AddResponseCompression();
            services.AddTransient<IToDoRepository, EFToDoRepository>();
            services.AddDbContext<ToDoContext>(options => 
                options.UseSqlServer(Configuration["Data:ToDoNetCore:ConnectionString"]));
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["Data:ToDoNetCore:ConnectionString"]));
            services.AddSingleton<ApplicationUptime>();
            services.Configure<ApplicationConfigurations>(Configuration.GetSection("ApplicationConfigurations"));
            services.AddIdentity<AppUser, IdentityRole>(opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Account/Login");
            services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordValidator>();

            // CORS added for Angular UI
            services.AddCors(
                options => options.AddPolicy(
                    DefaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            "http://localhost:4200"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsProduction())
            {
                loggerFactory.AddFile("ToDo_{Date}.txt");
            }

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseMiddleware<StatusCodeHandlingMiddleware>();
            app.UseAuthentication();
            app.UseResponseCompression();

            app.UseMvc(controller =>
            {
                controller.MapRoute(
                    "ViewOneToDo",
                    "ToDos/ID-{id:int}",
                    new {controller = "ToDo", action = "ViewOneItem"});

                controller.MapRoute(
                    "default", 
                    "{controller}/{action}/{id?}",
                    new {controller = "ToDo", action = "List"});
            });
        }
    }
}

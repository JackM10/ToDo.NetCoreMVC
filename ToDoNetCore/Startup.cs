using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoNetCore.Infrastructure;
using ToDoNetCore.Infrastructure.Cache;
using Microsoft.Extensions.Hosting;

namespace ToDoNetCore
{
    public class Startup
    {
        public AppSettings AppSettings { get; set; }
        public IConfiguration Configuration { get; }
        private const string DefaultCorsPolicyName = "localhost";

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = (new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()).Build();
            AppSettings = configuration.Get<AppSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddRazorPages();
            //services.AddMvc(options => options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName)));
            services.AddMvc();
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
            services.AddSingleton<ToDoMemCache>();

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
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
            app.UseRouting();
            app.UseCors("default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseMiddleware<StatusCodeHandlingMiddleware>();
            app.UseResponseCompression();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllerRoute("ViewOneToDo", "ToDos/ID-{id:int}",
                    new {controller = "ToDo", action = "ViewOneItem"});
                endpoint.MapControllerRoute("default", "{controller}/{action}/{id?}",
                    new { controller = "ToDo", action = "List" });
                endpoint.MapRazorPages();
            });
        }
    }
}

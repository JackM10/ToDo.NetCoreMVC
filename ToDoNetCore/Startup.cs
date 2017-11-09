using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;
using Microsoft.EntityFrameworkCore;
using ToDoNetCore.Controllers;

namespace ToDoNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCompression();

            var connection =
                @"Server=(localdb)\MSSQLLocalDB;Database=ToDoNetCore;Trusted_Connection=True;";
            services.AddDbContext<ToDoContext>(options => options.UseSqlServer(connection));
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(controller =>
            {
                controller.MapRoute("default", "{controller}/{action}/{id?}",
                    new {controller = "ToDo", action = "List"}); controller.MapRoute("view", "{id:int}", new { controller = "ToDo", action = "ViewOneItem" }); });
            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("app.UseMVC skipped - wrong controller selected!");
            });

            app.UseResponseCompression();
            loggerFactory.AddFile("ToDo_{Date}.txt");
        }
    }
}

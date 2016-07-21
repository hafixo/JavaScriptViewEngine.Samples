﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JavaScriptViewEngine.Pool;
using JavaScriptViewEngine;
using System.IO;

namespace Sample.MvcCore1
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsEngine(builder =>
            {
                builder.UsePooledEngineFactory(poolOptions =>
                {
                    poolOptions.WatchPath = Path.Combine(_env.WebRootPath, "dist");
                    poolOptions.WatchFiles = new List<string>
                    {
                        Path.Combine(poolOptions.WatchPath, "server.js")
                    };
                });
                builder.UseNodeRenderEngine(nodeOptions =>
                {
                    nodeOptions.ProjectDirectory = Path.Combine(_env.WebRootPath, "dist");
                    nodeOptions.GetModuleName = (path, model, bag, values, area, type) => "server";
                });
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseJsEngine(); // this needs to be before MVC

            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

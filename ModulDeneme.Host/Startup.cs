using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ModulDeneme.Host
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().ConfigureApplicationPartManager(ConfigureApplicationParts);
        }

        private void ConfigureApplicationParts(ApplicationPartManager apm)
        {
            var rootPath = _webHostEnvironment.ContentRootPath;
            var pluginsPath = Path.Combine(rootPath, "Plugins");

            var assemblyFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);
            foreach (var assemblyFile in assemblyFiles)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);
                    if (assemblyFile.EndsWith(".Views.dll"))
                        foreach (var b in new CompiledRazorAssemblyApplicationPartFactory().GetApplicationParts(assembly))
                            apm.ApplicationParts.Add(b);
                    else
                        apm.ApplicationParts.Add(new AssemblyPart(assembly));
                }
                catch (Exception e) { }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
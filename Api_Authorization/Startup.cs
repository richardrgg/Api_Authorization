using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Authorization.Middlewares;
using Api_Authorization.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api_Authorization
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                                .SetBasePath(env.ContentRootPath)
                                .AddJsonFile("appsettings.json")
                                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => Configuration);
            services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("Db_Demo"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.ConfigureJwt(Configuration["TokenKey"]);
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSwagger();
            app.ConfigureSwagerUI();
            app.UseMvc();
        }
    }
}

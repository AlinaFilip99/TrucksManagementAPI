using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Repositories;
using TrucksManagement.Services;

namespace TrucksManagement
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
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.User.RequireUniqueEmail = true;
            });

            var databaseProvider = Configuration["DatabaseProvider"];
            var connection = Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("The DefaultConnection connection string is not configured.");
            }

            services.AddDbContext<ApplicationContext>(options =>
            {
                if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlite(connection);
                }
                else if (string.Equals(databaseProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlServer(connection);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported database provider '{databaseProvider}'.");
                }
            });

            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<TripService>();
            services.AddScoped<UserService>();

            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrucksManagement", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                    database.Database.EnsureCreated();
                }

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { 
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrucksManagement v1"); 
                    c.RoutePrefix = ""; 
                });
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors(action => action.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

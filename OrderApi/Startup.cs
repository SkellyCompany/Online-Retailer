using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Services;

namespace OrderApi
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
            // In-memory database:
            services.AddDbContext<OrderApiContext>(opt => opt.UseInMemoryDatabase("OrdersDb"));

            // Register settings for dependency injection
            services.Configure<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));

            services.AddSingleton<IEmailSettings, EmailSettings>(sp =>
                sp.GetRequiredService<IOptions<EmailSettings>>().Value);

            // Register services for dependency injection
            services.AddScoped<IEmailService, EmailService>();

            // Register repositories for dependency injection
            services.AddScoped<IRepository<Order>, OrderRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Order API",
                    Description = "Swagger for Order API - Microservice Mini Project",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Initialize the database
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // Initialize the database
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<OrderApiContext>();
                var dbInitializer = services.GetService<IDbInitializer>();
                dbInitializer.Initialize(dbContext);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
            });
        }
    }
}

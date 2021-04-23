using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineRetailer.CustomerAPI.Core.ApplicationServices;
using OnlineRetailer.CustomerAPI.Core.ApplicationServices.Services;
using OnlineRetailer.CustomerAPI.Core.DomainServices;
using OnlineRetailer.CustomerAPI.Infrastructure.Database;
using OnlineRetailer.CustomerAPI.Infrastructure.Repositories;
using Prometheus;

namespace OnlineRetailer.CustomerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // In-memory database:
            services.AddDbContext<CustomerContext>(opt => opt.UseInMemoryDatabase("CustomersDb"));

            // Register services for dependency injection
            services.AddScoped<ICustomerService, CustomerService>();

            // Register repositories for dependency injection
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddControllers();

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Customer API",
                    Description = "Swagger for Customer API - Online Retailer",
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
                var dbContext = services.GetService<CustomerContext>();
                var dbInitializer = services.GetService<IDbInitializer>();
                dbInitializer.Initialize(dbContext);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API");
            });
        }
    }
}

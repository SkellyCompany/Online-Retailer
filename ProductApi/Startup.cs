using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineRetailer.ProductApi.Infrastructure.Database;
using OnlineRetailer.ProductApi.Infrastructure;
using OnlineRetailer.Entities;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using OnlineRetailer.Messaging;

namespace OnlineRetailer.ProductApi
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
            services.AddDbContext<ProductApiContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));

            // Register settings for dependency injection
            services.Configure<MessagingSettings>(Configuration.GetSection(nameof(MessagingSettings)));

            services.AddSingleton<IMessagingSettings, MessagingSettings>(sp =>
                            sp.GetRequiredService<IOptions<MessagingSettings>>().Value);

            // Register services for dependency injection
            services.AddScoped<IMessagingService, MessagingService>();

            // Register repositories for dependency injection
            services.AddScoped<IRepository<Product>, ProductRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddControllers();

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Product API",
                    Description = "Swagger for Product API - Microservice Mini Project",
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
                var dbContext = services.GetService<ProductApiContext>();
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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API");
            });

            Task.Factory.StartNew(() =>
            {
                new MessagingService(app.ApplicationServices.GetService<IOptions<MessagingSettings>>().Value).Subscribe("ass", "ass", (result) =>
                {
                    Debug.WriteLine("I got an assy message");
                });
            });
        }
    }
}

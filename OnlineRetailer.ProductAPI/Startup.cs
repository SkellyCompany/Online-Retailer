using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.ApplicationServices;
using OnlineRetailer.ProductAPI.Core.ApplicationServices.Services;
using OnlineRetailer.ProductAPI.Core.DomainServices;
using OnlineRetailer.ProductAPI.Core.Messaging.Receivers;
using OnlineRetailer.ProductAPI.Infrastructure.Database;
using OnlineRetailer.ProductAPI.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace OnlineRetailer.ProductAPI
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
            services.AddDbContext<ProductContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));

            // Register messaging components for dependency injection
            services.Configure<MessagingSettings>(Configuration.GetSection(nameof(MessagingSettings)));

            services.AddSingleton<IMessagingSettings, MessagingSettings>(sp =>
                sp.GetRequiredService<IOptions<MessagingSettings>>().Value);
            services.AddScoped<IMessagingService, MessagingService>();

            // Register services for dependency injection
            services.AddScoped<IProductService, ProductService>();

            // Register repositories for dependency injection
            services.AddScoped<IProductRepository, ProductRepository>();

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
                    Description = "Swagger for Product API - Online Retailer",
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
                var dbContext = services.GetService<ProductContext>();
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

            ConfigureSubscribers(app);
        }

        private void ConfigureSubscribers(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var settings = scope.ServiceProvider.GetService<IMessagingSettings>();
                new NewOrderSubscriber().Start(app, settings);
                new DeliveredOrderSubscriber().Start(app, settings);
                new CancelledOrderSubscriber().Start(app, settings);
            }
        }
    }
}

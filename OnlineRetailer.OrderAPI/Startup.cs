using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnlineRetailer.Messaging;
using OnlineRetailer.OrderAPI.Core.ApplicationServices;
using OnlineRetailer.OrderAPI.Core.ApplicationServices.Services;
using OnlineRetailer.OrderAPI.Core.DomainServices;
using OnlineRetailer.OrderAPI.Infrastructure.Database;
using OnlineRetailer.OrderAPI.Infrastructure.Repositories;
using Prometheus;

namespace OnlineRetailer.OrderAPI
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
            services.AddDbContext<OrderContext>(opt => opt.UseInMemoryDatabase("OrdersDb"));

            // Register messaging components for dependency injection
            services.Configure<MessagingSettings>(Configuration.GetSection(nameof(MessagingSettings)));

            services.AddSingleton<IMessagingSettings, MessagingSettings>(sp =>
                sp.GetRequiredService<IOptions<MessagingSettings>>().Value);
            services.AddScoped<IMessagingService, MessagingService>();

            // Register services for dependency injection
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IEmailService, EmailService>();

            // Register repositories for dependency injection
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Order API",
                    Description = "Swagger for Order API - Online Retailer",
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
                var dbContext = services.GetService<OrderContext>();
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
                endpoints.MapMetrics();

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

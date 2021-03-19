using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.ApplicationServices;
using OnlineRetailer.ProductAPI.Core.ApplicationServices.Services;
using OnlineRetailer.ProductAPI.Core.DomainServices;
using OnlineRetailer.ProductAPI.Core.Messaging.Receivers;
using OnlineRetailer.ProductAPI.Infrastructure.Database;
using OnlineRetailer.ProductAPI.Infrastructure.Repositories;
using System;
using System.Diagnostics;
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

            // Register services for dependency injection
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IMessagingService, MessagingService>();

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

            ConfigureReceivers(app);
        }

        private void ConfigureReceivers(IApplicationBuilder app)
        {
            MessagingSettings settings = new MessagingSettings { ConnectionString = "host=hawk.rmq.cloudamqp.com;virtualHost=qsqurewb;username=qsqurewb;password=UyeOEGtcb6zNFOvv_c3Pi-tZoEHJHgVb" };
            new NewOrderReceiver().Start(app, settings);
            new DeliveredOrderReceiver().Start(app, settings);
            new CancelledOrderReceiver().Start(app, settings);
        }

        private void ObserveNewOrders(IApplicationBuilder app)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    services.GetService<IMessagingService>().Receive("newOrder", (result) =>
                    {
                        if (result is Order)
                        {
                            Order order = result as Order;
                            var productRepository = services.GetService<IProductRepository>();
                            foreach (OrderLine line in order.OrderLines)
                            {
                                Product product = productRepository.Get(line.ProductId);
                                if (product != null)
                                {
                                    product.ItemsReserved += line.Quantity;
                                    productRepository.Edit(product);
                                }
                            }

                        }
                    });
                }
            });
        }

        private void ObserveDeliveredOrders(IApplicationBuilder app)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    services.GetService<IMessagingService>().Receive("deliveredOrder", (result) =>
                    {
                        if (result is Order)
                        {
                            Order order = result as Order;


                            var productRepository = services.GetService<IProductRepository>();
                            foreach (OrderLine line in order.OrderLines)
                            {
                                Product product = productRepository.Get(line.ProductId);
                                if (product != null)
                                {
                                    product.ItemsReserved -= line.Quantity;
                                    product.ItemsInStock -= line.Quantity;
                                    productRepository.Edit(product);
                                }
                            }
                        }

                    });
                }
            });
        }

        private void ObserveCancelledOrders(IApplicationBuilder app)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    services.GetService<IMessagingService>().Receive("cancelledOrder", (result) =>
                    {
                        if (result is Order)
                        {
                            Order order = result as Order;
                            var productRepository = services.GetService<IProductRepository>();
                            foreach (OrderLine line in order.OrderLines)
                            {
                                Product product = productRepository.Get(line.ProductId);
                                if (product != null)
                                {
                                    product.ItemsReserved -= line.Quantity;
                                    productRepository.Edit(product);
                                }
                            }
                        }
                    });
                }
            });
        }
    }
}

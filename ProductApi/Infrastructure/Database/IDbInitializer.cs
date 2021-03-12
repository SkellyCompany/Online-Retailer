namespace OnlineRetailer.ProductApi.Infrastructure.Database
{
    public interface IDbInitializer
    {
        void Initialize(ProductApiContext context);
    }
}

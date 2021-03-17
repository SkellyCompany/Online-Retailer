namespace OnlineRetailer.ProductAPI.Infrastructure.Database
{
    public interface IDbInitializer
    {
        void Initialize(ProductContext context);
    }
}

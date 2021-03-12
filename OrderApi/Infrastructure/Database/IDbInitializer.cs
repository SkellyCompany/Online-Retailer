namespace OnlineRetailer.OrderApi.Infrastructure.Database
{
    public interface IDbInitializer
    {
        void Initialize(OrderApiContext context);
    }
}

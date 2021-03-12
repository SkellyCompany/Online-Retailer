namespace OnlineRetailer.CustomerApi.Infrastructure.Database
{
    public interface IDbInitializer
    {
        void Initialize(CustomerApiContext context);
    }
}

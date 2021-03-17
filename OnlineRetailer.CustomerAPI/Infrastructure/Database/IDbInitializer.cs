namespace OnlineRetailer.CustomerAPI.Infrastructure.Database
{
    public interface IDbInitializer
    {
        void Initialize(CustomerContext context);
    }
}

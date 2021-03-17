namespace OnlineRetailer.OrderAPI.Infrastructure.Database
{
	public interface IDbInitializer
	{
		void Initialize(OrderContext context);
	}
}

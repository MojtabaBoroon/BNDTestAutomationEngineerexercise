using Internal.Services.Movements.Data.Contexts;
using Internal.Services.Movements.Data.Models;

namespace Internal.Services.Movements.IntegrationTests.Utilities
{
    public class DbHelper
	{
		private readonly MovementsDataContext _movementsDb;

		public DbHelper(MovementsDataContext movementsDb)
		{
			_movementsDb = movementsDb;
		}

		public void InitializeDbForTests()
		{
            _movementsDb.Products.Add(
                new Product
                {
                    ProductId = 1,
                    ProductType = Data.Models.Enums.EnumProductType.SavingsRetirement,
                    ExternalAccount = AccountHelper.CustomerAccount
                });

            _movementsDb.Products.Add(
                new Product
                {
                    ProductId = 2,
                    ProductType = Data.Models.Enums.EnumProductType.SavingsRetirement,
                    ExternalAccount = "NL54FAKE0062046222"
                });

            _movementsDb.SaveChanges(true);
		}
    }
}

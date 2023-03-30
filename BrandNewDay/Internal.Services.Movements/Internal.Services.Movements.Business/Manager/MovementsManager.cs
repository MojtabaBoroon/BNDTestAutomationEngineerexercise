using ModelEnums = Internal.Services.Movements.Data.Models.Enums;
using Internal.Services.Movements.ProxyClients;
using Internal.Services.Movements.Data.Contexts;

namespace Internal.Services.Movements.Business.Manager
{
    public class MovementsManager : IMovementsManager
    {
        const string FiscalTransferAccount = "NL54FAKE0326806738";
        private readonly IMovementsClient _movementsClient;
        private readonly MovementsDataContext _db;


        public MovementsManager(IMovementsClient movementsClient, MovementsDataContext db)
        {
            _movementsClient = movementsClient;
            _db = db;
        }

        public async Task<PagedMovements> GetMovements(int productId, ModelEnums.EnumMovementType? movementType, int pageNumber, int pageSize)
        {
            string? customerExternalAccount = getCustomerExternalAccount(productId);
            switch (movementType)
            {
                case ModelEnums.EnumMovementType.Fee:
                case ModelEnums.EnumMovementType.Interest:
                case ModelEnums.EnumMovementType.Tax:
                    return await _movementsClient.GetMovementsAsync(pageNumber, pageSize, customerExternalAccount, (EnumMovementType)movementType, null, null, null, null);
                case ModelEnums.EnumMovementType.Incoming:
                    return await _movementsClient.GetMovementsAsync(pageNumber, pageSize, customerExternalAccount, null, null, null, 0, null);
                case ModelEnums.EnumMovementType.Outgoing:
                    return await _movementsClient.GetMovementsAsync(pageNumber, pageSize, customerExternalAccount, null, null, null, null, 0);
                case ModelEnums.EnumMovementType.FiscalTransfer:
                    return await _movementsClient.GetMovementsAsync(pageNumber, pageSize, customerExternalAccount, null, FiscalTransferAccount, null, null, null);
                default:
                    return await _movementsClient.GetMovementsAsync(pageNumber, pageSize, customerExternalAccount, null, null, null, null, null);
            }
        }

        private string? getCustomerExternalAccount(int productId)
        {
            var product = _db.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (product == null)
            {
                return null;
            }
            return product.ExternalAccount;
        }
    }
}

using ModelEnums = Internal.Services.Movements.Data.Models.Enums;
using Internal.Services.Movements.ProxyClients;

namespace Internal.Services.Movements.Business.Manager
{
    public interface IMovementsManager
    {
        public Task<PagedMovements> GetMovements(int productId, ModelEnums.EnumMovementType? movementType, int pageNumber, int pageSize);
    }
}
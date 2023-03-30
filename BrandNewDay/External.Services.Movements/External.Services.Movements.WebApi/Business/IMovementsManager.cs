namespace External.Services.Movements.WebApi.Business
{
    public interface IMovementsManager
    {
        public PagedMovements GetMovements(int pageNumber, int pageSize, string? account, EnumMovementType? movementType, string? accountFrom, string? accountTo, decimal? amountFrom, decimal? amountTo);
    }
}

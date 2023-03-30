using Microsoft.AspNetCore.Mvc;
using External.Services.Movements.WebApi.Business;

namespace External.Services.Movements.WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly ILogger<MovementsController> _logger;
        private readonly IMovementsManager _movementManger;

        public MovementsController(ILogger<MovementsController> logger, IMovementsManager movementManager)
        {
            _logger = logger;
            _movementManger = movementManager;
        }

        [HttpGet(Name = "GetMovements")]
        public PagedMovements GetMovements(int pageNumber, int pageSize, string? account, EnumMovementType? movementType, string? accountFrom, string? accountTo, decimal? amountFrom, decimal? amountTo)
        {
            return _movementManger.GetMovements(pageNumber, pageSize, account, movementType, accountFrom, accountTo, amountFrom, amountTo);
        }
    }
}
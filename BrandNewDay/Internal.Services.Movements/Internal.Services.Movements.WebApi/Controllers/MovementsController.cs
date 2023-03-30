using Microsoft.AspNetCore.Mvc;
using Internal.Services.Movements.ProxyClients;
using Internal.Services.Movements.Business.Manager;

namespace Internal.Services.Movements.WebApi.Controllers
{
    [Route("v{version:apiVersion}/")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly IMovementsManager _movementsManager;

        public MovementsController(IMovementsManager movementsManager)
        {
            _movementsManager = movementsManager;
        }

        [HttpGet]
        [Route("GetMovements")]
        public async Task<ActionResult<PagedMovements>> GetMovements(int productId, Data.Models.Enums.EnumMovementType? movementType, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                return await _movementsManager.GetMovements(productId, movementType, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Error occured, {ex.Message}");
            }
        }
    }
}

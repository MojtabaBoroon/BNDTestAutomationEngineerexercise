using Internal.Services.Movements.ProxyClients;
using Moq;

namespace Internal.Services.Movements.IntegrationTests.Utilities
{
    public class MoqHelper
    {

        private readonly Mock<IMovementsClient> _movementsMock;

        public MoqHelper(Mock<IMovementsClient> movementsMock)
        {
            _movementsMock = movementsMock;
        }

        public IMovementsClient Object()
        {
            return _movementsMock.Object;
        }

        public void Setup(PagedMovements responseContent)
        {
            _movementsMock.Setup(x => x.GetMovementsAsync(
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<EnumMovementType?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>())).ReturnsAsync(responseContent);
        }

        public void Verify(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                _movementsMock.Verify(x => x.GetMovementsAsync(
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.IsAny<EnumMovementType?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>()
                ), Times.Once);

            }
            else
            {
                _movementsMock.Verify(x => x.GetMovementsAsync(
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    account,
                    It.IsAny<EnumMovementType?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>()
                ), Times.Once);
            }
        }
    }
}
using Internal.Services.Movements.IntegrationTests.Utilities;
using Internal.Services.Movements.ProxyClients;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using EnumMovementType = Internal.Services.Movements.Data.Models.Enums.EnumMovementType;
using ProxyEnumMovementType = Internal.Services.Movements.ProxyClients.EnumMovementType;

namespace Internal.Services.Movements.IntegrationTests
{
    public class IntegrationTest : IClassFixture<TestStartup<Program>>
    {
        private readonly TestStartup<Program> _factory;
        private readonly Utilities.MoqHelper _moq;
        private readonly HttpClient _client;

        private List<Movement> _movements = new();

        public IntegrationTest(TestStartup<Program> factory)
        {
            _factory = factory;
            _moq = new Utilities.MoqHelper(factory.MovementMock);
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_moq.Object());
                });
            }).CreateClient();
            _movements = CreateMovements();
        }

        private List<Movement> CreateMovements()
        {
            var movements = new List<Movement>();

            var customerAccount = AccountHelper.CustomerAccount;
            var customerNominatedAccount = AccountHelper.CustomerNominatedAccount;
            var interestAccount = AccountHelper.InterestAccount;
            var feeAccount = AccountHelper.FeeAccount;
            var taxAccount = AccountHelper.TaxAccount;
            var fiscalTransferAccount = AccountHelper.FiscalTransferAccount;
            for (int i = 0; i < 10; i++)
            {
                var newMovements = new List<Movement>
                {
                    // System interest movement
                    new Movement
                    {
                        MovementId = i * 6 + 1000,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Interest,
                        Amount = (decimal)0.42 + i,
                        AccountFrom = interestAccount,
                        AccountTo = customerAccount
                    },
                    // System fee movement
                    new Movement
                    {
                        MovementId = i * 6 + 1001,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Fee,
                        Amount = (decimal)-0.59 - i,
                        AccountFrom = customerAccount,
                        AccountTo = feeAccount
                    },
                    // System tax movement
                    new Movement
                    {
                        MovementId = i * 6 + 1002,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Tax,
                        Amount = (decimal)-200.77 - i,
                        AccountFrom = customerAccount,
                        AccountTo = taxAccount
                    },
                    // Fiscal transfer movement
                    new Movement
                    {
                        MovementId = i * 6 + 1003,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Unknown,
                        Amount = (decimal)17000 + i,
                        AccountFrom = fiscalTransferAccount,
                        AccountTo = customerAccount
                    },
                    // Incoming movement
                    new Movement
                    {
                        MovementId = i * 6 + 1004,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Interest,
                        Amount = (decimal)500 + i,
                        AccountFrom = customerNominatedAccount,
                        AccountTo = customerAccount
                    },
                    // Outgoing movement
                    new Movement
                    {
                        MovementId = i * 6 + 1005,
                        Account = customerAccount,
                        MovementType = ProxyEnumMovementType.Interest,
                        Amount = (decimal)-700 - i,
                        AccountFrom = customerAccount,
                        AccountTo = customerNominatedAccount
                    }
                };
                movements.AddRange(newMovements);
            }

            return movements;
        }

        [Fact]
        public async Task GetMovements_InternalApiIsCalled_ThirdPartyApiIsCalled()
        {
            // Arrang
            var responseContent = new PagedMovements();

            _moq.Setup(responseContent);

            // Act
            await _client.GetAsync($"/v1.0/GetMovements");

            //Assert
            _moq.Verify("");
        }

        [Theory]
        [InlineData(1, AccountHelper.CustomerAccount, EnumMovementType.Interest, 1, 5)]
        public async Task GetMovements_InternalApiIsCalled_AcocuntNumberIsFetched(int productId, string accountNumber, EnumMovementType movementType, int pageNumber, int pageSize)
        {
            // Arrang
            var pagedMovements = new PagedMovements();
            _moq.Setup(pagedMovements);

            // Act
            await _client.GetAsync($"/v1.0/GetMovements?productId={productId}&movementType={movementType}&pageNumber={pageNumber}&pageSize={pageSize}");

            //Assert
            _moq.Verify(accountNumber);
        }

        [Theory]
        [InlineData(1, AccountHelper.CustomerAccount,  EnumMovementType.Interest, 1, 5)]
        public async Task GetMovements_InterestMovementsWanted_InterestPaymentsFetched(int productId, string accountNumber, EnumMovementType movementType, int pageNumber, int pageSize)
        {
            // Arrang
            var pagedMovements = new PagedMovements
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Movements = _movements
                            .Where(x => x.Account == accountNumber)
                            .Where(x => x.MovementType == (ProxyEnumMovementType)movementType)
                            .ToList()
            };
            _moq.Setup(pagedMovements);

            // Act
            var response = await _client.GetAsync($"/v1.0/GetMovements?productId={productId}&movementType={movementType}&pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PagedMovements>(responseContent);
            Assert.Equal(pagedMovements.PageSize, responseModel.PageSize);
            Assert.Equal(pagedMovements.PageNumber, responseModel.PageNumber);
            Assert.Equal(pagedMovements.Movements.Count, responseModel.Movements.Count);
            Assert.Equal(accountNumber, responseModel.Movements.FirstOrDefault().Account);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementType, responseModel.Movements.FirstOrDefault().MovementType);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementId, responseModel.Movements.FirstOrDefault().MovementId);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountFrom, responseModel.Movements.FirstOrDefault().AccountFrom);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountTo, responseModel.Movements.FirstOrDefault().AccountTo);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().Amount, responseModel.Movements.FirstOrDefault().Amount);
        }

        [Theory]
        [InlineData(1, AccountHelper.CustomerAccount, EnumMovementType.Incoming, 1, 5)]
        public async Task GetMovements_IncomingMovementsWanted_IncomingPaymentsFetched(int productId, string accountNumber, EnumMovementType movementType, int pageNumber, int pageSize)
        {
            // Arrang
            var fiscalTransferAccount = AccountHelper.FiscalTransferAccount;
            var pagedMovements = new PagedMovements
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Movements = _movements
                            .Where(x => x.Account == accountNumber)
                            .Where(x => x.AccountFrom == fiscalTransferAccount)
                            .ToList()
            };

            _moq.Setup(pagedMovements);

            // Act
            var response = await _client.GetAsync($"/v1.0/GetMovements?productId={productId}&movementType={movementType}&pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PagedMovements>(responseContent);
            Assert.Equal(pagedMovements.PageSize, responseModel.PageSize);
            Assert.Equal(pagedMovements.PageNumber, responseModel.PageNumber);
            Assert.Equal(pagedMovements.Movements.Count, responseModel.Movements.Count);
            Assert.Equal(accountNumber, responseModel.Movements.FirstOrDefault().Account);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementType, responseModel.Movements.FirstOrDefault().MovementType);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementId, responseModel.Movements.FirstOrDefault().MovementId);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountFrom, responseModel.Movements.FirstOrDefault().AccountFrom);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountTo, responseModel.Movements.FirstOrDefault().AccountTo);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().Amount, responseModel.Movements.FirstOrDefault().Amount);
        }

        [Theory]
        [InlineData(1, AccountHelper.CustomerAccount, EnumMovementType.FiscalTransfer, 1, 5)]
        public async Task GetMovements_FiscalTransferMovementsWanted_FiscalTransferPaymentsFetched(int productId, string accountNumber, EnumMovementType movementType, int pageNumber, int pageSize)
        {
            // Arrang
            var pagedMovements = new PagedMovements
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Movements = _movements
                            .Where(x => x.Account == accountNumber)
                            .Where(x => x.Amount >= 0)
                            .ToList()
            };

            _moq.Setup(pagedMovements);

            // Act
            var response = await _client.GetAsync($"/v1.0/GetMovements?productId={productId}&movementType={movementType}&pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PagedMovements>(responseContent);
            Assert.Equal(pagedMovements.PageSize, responseModel.PageSize);
            Assert.Equal(pagedMovements.PageNumber, responseModel.PageNumber);
            Assert.Equal(pagedMovements.Movements.Count, responseModel.Movements.Count);
            Assert.Equal(accountNumber, responseModel.Movements.FirstOrDefault().Account);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementType, responseModel.Movements.FirstOrDefault().MovementType);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().MovementId, responseModel.Movements.FirstOrDefault().MovementId);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountFrom, responseModel.Movements.FirstOrDefault().AccountFrom);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().AccountTo, responseModel.Movements.FirstOrDefault().AccountTo);
            Assert.Equal(pagedMovements.Movements.FirstOrDefault().Amount, responseModel.Movements.FirstOrDefault().Amount);
        }
    }
}
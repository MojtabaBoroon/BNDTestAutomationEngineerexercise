using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Internal.Services.Movements.Data.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Internal.Services.Movements.ProxyClients;
using Moq;

namespace Internal.Services.Movements.IntegrationTests
{
    public class TestStartup<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public TestStartup()
        {
            MovementMock = new Mock<IMovementsClient>();
        }

        public Mock<IMovementsClient> MovementMock { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptorDb = services.Single(d => d.ServiceType == typeof(DbContextOptions<MovementsDataContext>));
                var descriptorExternalMovements = services.Single(d => d.ServiceType == typeof(IMovementsClient));

                services.Remove(descriptorDb);
                services.Remove(descriptorExternalMovements);

                services.AddDbContext<MovementsDataContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
                services.AddSingleton(MovementMock.Object);

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MovementsDataContext>();
                    db.Database.EnsureCreated();

                    var dbHelper = new Utilities.DbHelper(db);
                    dbHelper.InitializeDbForTests();
                }

            });
            base.ConfigureWebHost(builder);
        }
    }
}

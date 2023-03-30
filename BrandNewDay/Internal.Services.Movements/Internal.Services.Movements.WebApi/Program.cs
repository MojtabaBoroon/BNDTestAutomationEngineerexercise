using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Internal.Services.Movements.ProxyClients;
using Internal.Services.Movements.Business.Manager;
using Internal.Services.Movements.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

//  Adding controllers and making sure the json is formatted correctly
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(
                        opts =>
                        {
                            opts.SerializerSettings.Formatting = Formatting.Indented;
                            opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                            opts.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                            opts.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                            opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                            opts.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                            opts.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                            opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            opts.SerializerSettings.Culture = CultureInfo.InvariantCulture;
                        });

// Adding the OpenApi documents
builder.Services.AddOpenApiDocument(
                opts =>
                {
                    opts.AllowReferencesWithProperties = true;
                    opts.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "Internal.Services.Movements.WebApi";
                        document.Info.License = new NSwag.OpenApiLicense { Name = "CLPT", Url = "https://youtu.be/E8gmARGvPlI" };
                    };
                });
// Setting the version of the Api
builder.Services.AddApiVersioning(o =>
{
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
})
                .AddVersionedApiExplorer(opts =>
                {
                    opts.SubstituteApiVersionInUrl = true;
                });

// Adding ProxyClients which than can be used by other controllers
HttpClientHandler clientHandler = new();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

builder.Services.AddHttpClient<IMovementsClient, MovementsClient>((sp, c) =>
{
    c.BaseAddress = new Uri("https://localhost:49153");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
})
.SetHandlerLifetime(TimeSpan.FromMinutes(2));

// Adding the interfaces for the controllers
builder.Services.AddScoped<IMovementsManager, MovementsManager>();

// Adding the Db connection
builder.Services.AddDbContext<MovementsDataContext>(options => options.UseSqlite("Filename=MovementsTest.db"));

var app = builder.Build();

// Migrating all the changes to the db
var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<MovementsDataContext>();
context.Database.EnsureCreated();

// Using Nswag to have proper EnumStringConversion in the requests
app.UseOpenApi();
app.UseSwaggerUi3(s =>
{
    s.EnableTryItOut = true;
    s.ValidateSpecification = true;
    s.WithCredentials = true;
    s.OperationsSorter = "method";  // "alpha", "method"
    s.ValidateSpecification = false;
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
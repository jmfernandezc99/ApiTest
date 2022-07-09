var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"src/ApiTest.Api/appsettings.development.json");
var _configuration = configuration.Build();

var foo = _configuration.GetValue<string>("redisUrl");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = _configuration.GetValue<string>("redisUrl");
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

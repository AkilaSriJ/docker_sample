using Genx.TrainTatkalBooking.Api.DependencyInjection;
using Genx.TrainTatkalBooking.Data.Context;
using Genx.TrainTatkalBooking.Service.Mapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Get connection string (priority: Environment Variable > appsettings.json)
var connFromEnv = Environment.GetEnvironmentVariable("MYSQLCONN");
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
var mysqlConn = string.IsNullOrWhiteSpace(connFromEnv) ? defaultConn : connFromEnv;

// ✅ Log for debugging (optional — remove after confirming)
Console.WriteLine($"Using MySQL connection: {mysqlConn}");

// ✅ Configure DbContext for Aiven MySQL (MariaDB 11.5.x compatible)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        mysqlConn,
        new MySqlServerVersion(new Version(11, 5, 2)),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    )
);

// ✅ Register dependencies
builder.Services.AddDataDI();
builder.Services.AddServiceDI();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ✅ Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Enable CORS (for React frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://frontend:80")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ✅ Configure port (Render assigns dynamically)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.UseCors("AllowReactApp");

// ✅ Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

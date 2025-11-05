using Genx.TrainTatkalBooking.Api.DependencyInjection;
using Genx.TrainTatkalBooking.Data.Context;
using Genx.TrainTatkalBooking.Service.Mapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connFromEnv = Environment.GetEnvironmentVariable("MYSQL_CONNECTION");
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
var mysqlConn = string.IsNullOrWhiteSpace(connFromEnv) ? defaultConn : connFromEnv;

// Configure DbContext for MariaDB 11.5.2
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        mysqlConn,
        new MySqlServerVersion(new Version(11, 5, 2)),   // ✅ Match your MariaDB version
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); // ✅ adds retry resilience
        }
    )
);
builder.Services.AddDataDI();
builder.Services.AddServiceDI();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
var port = Environment.GetEnvironmentVariable("PORT") ?? "5193";
app.Urls.Add($"http://*:{port}");

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

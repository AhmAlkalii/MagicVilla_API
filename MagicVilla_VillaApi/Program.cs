
using MagicVilla_VillaApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//For logger services
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval:RollingInterval.Day).CreateLogger();

//tells the app to not use the defualt logger but the seri logger 
//builder.Host.UseSerilog();

//To add the db and connect
builder.Services.AddDbContext<ApplicationDbContext>(
    option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefualtSQLConnection"));
    });

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//in order for server to know our log class and interface
//builder.Services.AddSingleton<ILogging,Logging>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SupportChat.API.Extensions;
using SupportChat.API.Middleware;
using SupportChat.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SupportChat API",
        Version = "v1",
        Description = "A simple solution for chat support queue management."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SupportChatDbContext>();
    //Console.WriteLine($"API: SQLite connection string: {dbContext.Database.GetConnectionString()}");
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SupportChat API v1");
    options.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
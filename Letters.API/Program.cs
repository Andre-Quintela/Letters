using Letters.Infrastructure.Context;
using Letters.IOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddinfrastructureDependencies(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    bool canConnect = dbContext.Database.CanConnect();
    Console.WriteLine($"Conexão com o banco: {(canConnect ? "SUCESSO" : "FALHA")}");
}

app.Run();


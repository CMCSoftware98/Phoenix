using Google.Cloud.Firestore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFirestoreDb(x =>
{
    x.ProjectId = "phoenix-430818";
    x.DatabaseId = "phoenix-db";
    x.CredentialsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\auth.json";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

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
    x.JsonCredentials = "{\r\n  \"account\": \"\",\r\n  \"client_id\": \"764086051850-6qr4p6gpi6hn506pt8ejuq83di341hur.apps.googleusercontent.com\",\r\n  \"client_secret\": \"d-FL95Q19q7MQmFpd7hHD0Ty\",\r\n  \"quota_project_id\": \"phoenix-430818\",\r\n  \"refresh_token\": \"1//03Nk0QEUc9xa1CgYIARAAGAMSNwF-L9Iro9xe_VukAGdzD4Z1VD1jpUabRCAPcM7CY59N_5W3NqbzBfEs2e9vl7nSk_NmXOq6ycI\",\r\n  \"type\": \"authorized_user\",\r\n  \"universe_domain\": \"googleapis.com\"\r\n}";
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

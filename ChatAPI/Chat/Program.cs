using Chat.Hubs;
using Contracts;
using Entities;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => 
    builder.WithOrigins("http://peabody28.com:5091")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
    b => b.MigrationsAssembly("Chat")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.UseEndpoints(configure: endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/api");
});

app.MapControllers();

app.Run();

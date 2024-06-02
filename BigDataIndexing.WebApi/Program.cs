using BigDataIndexing.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new RedisService("localhost"));
builder.Services.AddSingleton(new JsonValidator("/Users/athipathi/RiderProjects/BigDataIndexing/BigDataIndexing.WebApi/Schema/UseCaseSchema.json")); // Update the path to your JSON schema file
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

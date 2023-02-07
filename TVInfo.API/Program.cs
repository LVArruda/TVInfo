using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Models.Services;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<MongoDBSettings>(
builder.Configuration.GetSection("MongoDB"));

/*
Per the official Mongo Client reuse guidelines, MongoClient should be registered in DI with a singleton service lifetime.
https://mongodb.github.io/mongo-csharp-driver/2.14/reference/driver/connecting/#re-use
*/
builder.Services.AddSingleton<IMongoDBClient, MongoDBClient>();
builder.Services.AddTransient<ITVShowMongoDBService, TVShowMongoDBService>();

var app = builder.Build();

//TODO: AddHealthChecks

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();    
}
else
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");
        });
    });

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }

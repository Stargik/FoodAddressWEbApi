
using System.Reflection;
using FoodAddressWEbApi.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FoodAddressWEbApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddDbContext<FoodAddressDbContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("FoodAddressDbConnection"))
        );

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("ServiceSettings"));

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, configurator) =>
            {
                x.AddConsumers(Assembly.GetEntryAssembly());

                var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
                var serviceSettings = builder.Configuration.GetSection("ServiceSettings").Get<ServiceSettings>();
                configurator.Host(rabbitMQSettings.Host, (cfg) =>
                {
                    cfg.Username(rabbitMQSettings.User);
                    cfg.Password(rabbitMQSettings.Password);
                });
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
            });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        app.MigrateDatabase();

        app.Run();
    }
}
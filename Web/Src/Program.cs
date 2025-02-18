using Microsoft.OpenApi.Models;
using Infrastructure;
using Web.Src.Service;
using StackExchange.Redis;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(configuration!);
            });

            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Speed Test",
                    Version = "v1",
                    Description = "Api for testing download speed"

                });
                options.EnableAnnotations();

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Web.xml");
                options.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddScoped<Func<HttpClient>>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                return () => httpClientFactory.CreateClient();
            });

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IServerService, ServerService>();
            builder.Services.AddScoped<IPingService, PingService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<ISpeedTestService, SpeedTestService>();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IFileReader, FileReader>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.MapControllers();
            app.Run();
        }
    }
}

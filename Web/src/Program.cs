using Web.src.Servcie;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IServerService, ServerService>();
            builder.Services.AddScoped<IPingService, PingService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<ISpeedTestService, SpeedTestService>();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();
            app.Run();
        }
    }
}

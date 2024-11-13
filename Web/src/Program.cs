using Web.src.Servcie;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<ISpeedTestService, SpeedTestService>();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();
            app.Run();
        }
    }
}
using Simulation;
using Simulation.Maps;

namespace SimWeb;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Dodajemy Razor Pages
        builder.Services.AddRazorPages();

        // Inicjalizujemy map? i gracza
        var map = MapInitialization.InitializeMap();
        var player = MapInitialization.InitializePlayer(map);

        // Rejestrujemy GameSession jako singleton
        var session = new GameSession();
        session.Initialize(map, player);
        builder.Services.AddSingleton(session);

        var app = builder.Build();

        // Konfiguracja pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        app.Run();
    }
}
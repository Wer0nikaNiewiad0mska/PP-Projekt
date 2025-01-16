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

        // Inicjalizujemy mapy i gracza
        var (bigMap, secondMap, player, maps) = GameInitialization.InitializeGame();
        var session = new GameSession();
        session.Initialize(bigMap, player, maps);

        // Rejestrujemy GameSession jako singleton
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
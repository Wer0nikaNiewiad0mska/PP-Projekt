using Simulation;
using Simulation.Maps;

namespace SimWeb;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Dodajemy map? do DI
        var map = new BigMap(10, 10);
        MapRules.AddBlockedField(map.Fields, new Point(1, 1));
        MapRules.AddPotion(map.Fields, new Point(5, 5), "DoubleMovement");
        builder.Services.AddSingleton(map);

        // Dodanie GameSession jako singleton
        builder.Services.AddSingleton<GameSession>();

        // Dodaj Razor Pages
        builder.Services.AddRazorPages();

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
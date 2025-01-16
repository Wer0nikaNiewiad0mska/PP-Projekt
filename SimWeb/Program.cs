using Simulation;
using Simulation.Maps;

namespace SimWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDistributedMemoryCache(); // Obs�uga pami�ci sesji
        builder.Services.AddSession(); // Dodanie obs�ugi sesji

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // W��cz obs�ug� sesji
        app.UseSession();
        
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
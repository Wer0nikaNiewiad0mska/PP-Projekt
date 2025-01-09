using Simulation;
using Simulation.Maps;

namespace SimWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            //Bez tego nie dzia³a symulacja
            builder.Services.AddSingleton<BigMap>(sp =>
            {
                var map = new BigMap(20, 20);
                // Dodaj zablokowane pola, klucze, eliksiry itp. tutaj, jeœli chcesz
                return map;
            });

            builder.Services.AddSingleton<Player>(sp =>
            {
                var player = new Player("DefaultPlayer");
                var map = sp.GetRequiredService<BigMap>();
                player.InitMapAndPosition(map, new Point(0, 0));
                return player;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
}

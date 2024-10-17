using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();

        string? connectionString = "Data Source=" + Environment.GetEnvironmentVariable("CHIRPDBPATH");
        builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<ICheepService, CheepService>();

        var app = builder.Build();
    
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Database currently migrated and seeded on every start. 
        // Remove when data needs to be persistent on server. 
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            context.Database.Migrate();
            DbInitializer.SeedDatabase(context);
        }

        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapRazorPages();

        app.Run();
    }
    
}
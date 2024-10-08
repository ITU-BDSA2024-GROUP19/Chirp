using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string? connectionString = "Data Source=" + Environment.GetEnvironmentVariable("CHIRPDBPATH");
        builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();

        var app = builder.Build();
    
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // Create a disposable service scope
        using (var scope = app.Services.CreateScope())
        {
            // From the scope, get an instance of our database context.
            // Through the `using` keyword, we make sure to dispose it after we are done.
            using var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            
            // Execute the migration from code.
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
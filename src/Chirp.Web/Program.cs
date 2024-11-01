using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using InvalidOperationException = System.InvalidOperationException;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string ChirpDBPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? throw new InvalidOperationException("You must specify a environment variable CHIRPDBPATH, use eg. $env:CHIRPDBPATH=C:/Temp/db.db");
        string connectionString = "Data Source=" + ChirpDBPath;
        builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<ICheepService, CheepService>();
        builder.Services.AddRouting();
        builder.Services.AddAuthentication(options => 
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGitHub(o => 
        {
            o.ClientId = builder.Configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("You must provide a github clientId");
            o.ClientSecret = builder.Configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("You must provide a github clientSecret");
            o.CallbackPath = "/signin-github";
        });

        builder.Services.AddDefaultIdentity<Author>(options => 
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        builder.Services.AddRazorPages();

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
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
            context.Database.Migrate();
            DbInitializer.SeedDatabase(context, userManager);
        }

        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
    
}
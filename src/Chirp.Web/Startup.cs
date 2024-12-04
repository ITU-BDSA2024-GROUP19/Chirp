/*
 * The following methods are derived from the GitHub OAuth sample client:
 * https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/samples/Mvc.Client/Startup.cs
 * The class is adopted to help produce a cleaner code that is easier to test. 
 * It has been modified to suit the startup tasks of the Chirp web app. 
 *
 * Original header:
 *
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

public class Startup(IConfiguration configuration, SqliteConnection dbConn)
 {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(dbConn));

        services.AddScoped<ICheepRepository, CheepRepository>();

        services.AddScoped<ICheepService, CheepService>();

        services.AddScoped<IAuthorRepository, AuthorRepository>();

        services.AddScoped<IAuthorService, AuthorService>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        
        .AddCookie()

        .AddGitHub(options =>
        {
            options.ClientId = configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientId");
            options.ClientSecret = configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientSecret");
            options.CallbackPath = "/signin-github";
        });

        services.AddDefaultIdentity<Author>(options => 
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        services.AddRazorPages();
    }

    public async void Configure(WebApplication app) 
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
            
            context.Database.Migrate();

            // Seeds with default dataset if starting with an empty DB.
            if (!context.Authors.Any() && !context.Cheeps.Any())
            {
                var initializer = new DbInitializer(context, userManager);
                await initializer.SeedDatabase();
            }
        }

        app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
    }
 }
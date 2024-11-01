/*
 * The following methods are derived from the GitHub OAuth sample client:
 * https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/samples/Mvc.Client/Startup.cs
 * The methods have been adopted to help produce a cleaner code that is easier to test. 
 * They have been modified to suit the startup tasks of the Chirp web app. 
 *
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

public class Startup(IConfiguration configuration, string connectionString)
 {
    public void ConfigureServices(IServiceCollection services)
    {
        if (configuration == null) throw new NullReferenceException("Missing builder configuration");
        if (connectionString == null) throw new NullReferenceException("ConnectionString not set");

        services.AddRouting();

        services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<ICheepRepository, CheepRepository>();

        services.AddScoped<ICheepService, CheepService>();

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

    public void Configure(WebApplication app) 
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
    }
 }
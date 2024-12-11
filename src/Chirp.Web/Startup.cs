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

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    public WebApplication App { get; }

    public Startup(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        AddServices();
        App = _builder.Build();
        AddMiddleware().Wait();
    }

    public void AddServices()
    {
        _builder.Services.AddRouting();

        var dbPath = _builder.Configuration["CHIRPDBPATH"];
        if (dbPath == null && _builder.Environment.IsDevelopment())
        {
            Console.WriteLine("App will use an in-memory database");
            dbPath = ":memory:";
        }
        else if (dbPath == null && !_builder.Environment.IsDevelopment())
        {
            throw new InvalidOperationException("You must specify a environment variable CHIRPDBPATH, use eg. $env:CHIRPDBPATH=C:/Temp/db.db");
        }

        _builder.Services.AddSingleton<SqliteConnection>(_ => {
            var connection = new SqliteConnection("Data Source=" + dbPath);
            connection.Open();
            return connection;
        });

        _builder.Services.AddDbContext<ChirpDBContext>((serviceProvider, options) => {
            var connection = serviceProvider.GetRequiredService<SqliteConnection>();
            options.UseSqlite(connection);
        });

        _builder.Services.AddScoped<ICheepRepository, CheepRepository>();

        _builder.Services.AddScoped<ICheepService, CheepService>();

        _builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

        _builder.Services.AddScoped<IAuthorService, AuthorService>();

        _builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        
        .AddCookie()

        .AddGitHub(options =>
        {
            options.ClientId = _builder.Configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientId");
            options.ClientSecret = _builder.Configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientSecret");
            options.CallbackPath = "/signin-github";
            options.SaveTokens = true;
            options.Scope.Add("user:email");
        });

        _builder.Services.AddDefaultIdentity<Author>(options => 
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        _builder.Services.AddRazorPages();
    }

    public async Task AddMiddleware() 
    {
        if (!App.Environment.IsDevelopment())
        {
            App.UseExceptionHandler("/Error");
            App.UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        using (var scope = App.Services.CreateScope())
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

        App.UseDeveloperExceptionPage();

        App.UseHttpsRedirection();

        App.UseStaticFiles();

        App.UseRouting();

        App.UseAuthentication();
        App.UseAuthorization();

        App.MapRazorPages();
    }
}
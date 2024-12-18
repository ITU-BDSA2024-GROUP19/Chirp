/*
 * The following methods are derived from the GitHub OAuth sample client:
 * https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/samples/Mvc.Client/Startup.cs
 * The class has been adopted to help produce a cleaner code that is easier to test. 
 * It has been modified to suit the startup tasks of the Chirp web app. 
 *
 * Original header:
 *
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Azure.Storage.Blobs;

using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Authors;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.External;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

/// <summary>
/// Methods to build the services and middleware stack for Chirp.
/// </summary>
/// <param name="configuration"></param>
/// <param name="environment"></param>
public class Startup(IConfiguration configuration, IHostEnvironment environment)
{
    /// <summary>
    /// Configures the stack of all services used by Chirp. 
    /// Services are then obtained through dependency injection as required.
    /// </summary>
    /// <param name="services">Application service collection.</param>
    /// <exception cref="InvalidOperationException">If the environment is not setup for Chirp. In particular, if env. variable CHIRPDBPATH is missing.</exception>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        services.AddRazorPages();

        // Evaluate environment variable CHIRPDBPATH:
        string? dbPath = configuration["CHIRPDBPATH"];
        if (dbPath == null && !environment.IsDevelopment())
        {
            throw new InvalidOperationException("""
                                                No environment variable CHIRPDBPATH found in non-dev. environment.
                                                You must specify a environment variable CHIRPDBPATH, use eg. $env:CHIRPDBPATH=C:/Temp/db.db
                                                """);
        }
        else if (dbPath == null && environment.IsDevelopment())
        {
            Console.WriteLine("""
                              No environment variable CHIRPDBPATH found. - Dev. environment detected.
                              Will use in-memory SQLite with sample content. 
                              """);
            dbPath = ":memory:";
        }

        // This singleton pattern allows in-memory SQLite to work correctly.
        // From: https://www.answeroverflow.com/m/1071789602316238919
        services.AddSingleton<SqliteConnection>(_ =>
        {
            var connection = new SqliteConnection("Data Source=" + dbPath);
            connection.Open();
            return connection;
        });

        services.AddDbContext<ChirpDBContext>((serviceProvider, options) =>
        {
            var connection = serviceProvider.GetRequiredService<SqliteConnection>();
            options.UseSqlite(connection);
        });

        // Evaluate connection to Azure Blob Storage for profile picture support:
        string? connectionBlobString = configuration["azure:storage:connection:string"];

        if (connectionBlobString != null)
        {
            services.AddSingleton<IOptionalBlobServiceClient, OptionalBlobServiceClient>(x =>
                new OptionalBlobServiceClient(new BlobServiceClient(connectionBlobString)));
        }
        else
        {
            Console.WriteLine("""
                              Azure Blob Storage connection string not found.
                              Blob storage will not be available.
                              Profile pictures will use default fallback image.
                              """);
            services.AddSingleton<IOptionalBlobServiceClient, OptionalBlobServiceClient>(x =>
                new OptionalBlobServiceClient());
        }

        // Chirp infrastructure services:
        services.AddScoped<ICheepRepository, CheepRepository>();

        services.AddScoped<ICheepService, CheepService>();

        services.AddScoped<IAuthorRepository, AuthorRepository>();

        services.AddScoped<IAuthorService, AuthorService>();

        // Identity authentication services:
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })

        .AddCookie();

        string? github_clientid = configuration["authentication:github:clientId"];
        string? github_clientsecret = configuration["authentication:github:clientSecret"];

        // GitHub OAuth as optional feature:
        if (github_clientid != null && github_clientsecret != null)
        {
            authBuilder.AddGitHub(options =>
            {
                options.ClientId = configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientId");
                options.ClientSecret = configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientSecret");
                options.CallbackPath = "/signin-github";
                options.SaveTokens = true;
                options.Scope.Add("user:email");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
            });
        }
        else
        {
            Console.WriteLine("""
                              GitHub Client ID and Client Secret not found.
                              OAuth via GitHub will not be available.
                              """);
        }

        services.AddDefaultIdentity<Author>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ChirpDBContext>();
    }

    /// <summary>
    /// Configures the middleware stack for Chirp.
    /// </summary>
    /// <param name="app"></param>
    public async void Configure(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }
        else
        {
            app.UseDeveloperExceptionPage();
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

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
    }

}
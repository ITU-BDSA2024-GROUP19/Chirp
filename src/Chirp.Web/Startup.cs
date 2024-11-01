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
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web;

public class Startup
 {
    public IConfiguration ?Configuration {get; set;}
    public IWebHostEnvironment ?Environment {get; set;}
    public string ?ConnectionString {get; set;}

    public void ConfigureServices(IServiceCollection services)
    {
        if (Configuration == null) throw new NullReferenceException("Missing builder configuration");
        if (ConnectionString == null) throw new NullReferenceException("ConnectionString not set");

        services.AddRouting();

        services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(ConnectionString));

        services.AddScoped<ICheepRepository, CheepRepository>();

        services.AddScoped<ICheepService, CheepService>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        
        .AddCookie()

        .AddGitHub(options =>
        {
            options.ClientId = Configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientId");
            options.ClientSecret = Configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("Configuration missing a GitHub clientSecret");
            options.CallbackPath = "/signin-github";
        });

        services.AddDefaultIdentity<Author>(options => 
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        services.AddRazorPages();
    }
 }
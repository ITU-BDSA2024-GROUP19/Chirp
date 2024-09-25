﻿using Chirp.SimpleDB;
using DocoptNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chirp.Program;

public static class Program
{
    const string path = "../../data/chirp_cli_db.csv";

    public static async Task Main(string[] args)
    {
        var arguments = new Docopt().Apply(UserInterface.GetCLIMessage(), args, version: "0.1", exit: true);
        
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }

        if (arguments == null) return;
        
        if (arguments["read"].IsTrue)
        {
            CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
            UserInterface.PrintCheeps(db.Read(arguments["<limit>"].AsInt));
        }
        if (arguments["webRead"].IsTrue)
        {
            var baseURL = "https://bdsagroup19chirpremotedb.azurewebsites.net";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);
            int limit = arguments["<limit>"].AsInt;
            if (limit != 0)
            {
                await ProcessCheeps(client, limit);
            }
            else
            { 
                await ProcessCheeps(client);
            }
        }
        else if (arguments["cheep"].IsTrue)
        {
            StoreCheeps(arguments["<message>"].ToString());
        }
        else if (arguments["webCheep"].IsTrue)
        {
            await StoreWebCheeps(arguments["<message>"].ToString());
        }
        else if (arguments["bootLocalHost"].IsTrue)
        {
            //CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
            //var cheeps = db.Read(arguments["<limit>"].AsInt);
            app.MapGet("/readCheeps", () =>
            {
                CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
                var cheeps = db.Read(arguments["<limit>"].AsInt);
                return cheeps;
            });
            app.MapPost("/storeCheep", (Cheep cheep) =>
            {
                CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
                db.Store(cheep);
                return Results.Ok("Cheep posted successfully");
            });
            app.Run();
        }
    }

    private static void StoreCheeps(string message)
    {
        CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
        Cheep c = Cheep.NewCheep(message);
        db.Store(c);
        Console.WriteLine("Cheep posted successfully!");
    }

    private static async Task StoreWebCheeps(string message)
    {
        var baseURL = "https://bdsagroup19chirpremotedb.azurewebsites.net";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);
        Cheep cheep = Cheep.NewCheep(message);
        HttpResponseMessage response = await client.PostAsJsonAsync("/storeCheep", cheep);
        Console.WriteLine(response.IsSuccessStatusCode ? "Cheep posted successfully!" : "Cheep posted failed!");
    }
    

    private static async Task ProcessCheeps(HttpClient client)
    {
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("/readCheeps");
        if (cheeps is not null) UserInterface.PrintCheeps(cheeps);
    }
    
    private static async Task ProcessCheeps(HttpClient client, int limit)
    {
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("/readCheeps");
        for (int i = 0; i < limit && i < cheeps?.Count; i++)
        {
            Console.WriteLine(cheeps[i].ToString());
        }
    }
}
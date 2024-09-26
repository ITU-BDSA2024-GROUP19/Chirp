using Chirp.SimpleDB;
using DocoptNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chirp.Program;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var arguments = new Docopt().Apply(UserInterface.GetCLIMessage(), args, version: "0.1", exit: true);
        
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        var baseURL = "https://bdsagroup19chirpremotedb.azurewebsites.net";

        if (arguments == null) return;
        
        if (arguments["webRead"].IsTrue)
        {
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
        else if (arguments["webCheep"].IsTrue)
        {
            await StoreWebCheeps(arguments["<message>"].ToString(), baseURL);
        }
    }

    private static async Task StoreWebCheeps(string message, string baseURL)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);
        Cheep cheep = Cheep.NewCheep(message);
        HttpResponseMessage response = await client.PostAsJsonAsync("/cheep", cheep);
        Console.WriteLine(response.IsSuccessStatusCode ? "Cheep posted successfully!" : "Failed to post cheep!");
    }
    

    private static async Task ProcessCheeps(HttpClient client)
    {
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("/cheeps");
        if (cheeps is not null) UserInterface.PrintCheeps(cheeps);
    }
    
    private static async Task ProcessCheeps(HttpClient client, int limit)
    {
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("/cheeps");
        for (int i = 0; i < limit && i < cheeps?.Count; i++)
        {
            Console.WriteLine(cheeps[i].ToString());
        }
    }
}
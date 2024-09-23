using Chirp.SimpleDB;
using DocoptNet;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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
            var baseURL = "http://localhost:5000";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);
            await ProcessCheeps(client);
        }
        else if (arguments["cheep"].IsTrue)
        {
            StoreCheeps(arguments["<message>"].ToString());
        }
        else if (arguments["bootLocalHost"].IsTrue)
        {
            CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
            var cheeps = db.Read(arguments["<limit>"].AsInt);
            app.MapGet("/readCheeps", () => cheeps);
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

    private static async Task ProcessCheeps(HttpClient client)
    {
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("/readCheeps");
        UserInterface.PrintCheeps(cheeps);
    }
}
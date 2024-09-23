using Chirp.SimpleDB;
using DocoptNet;
using Microsoft.AspNetCore.Builder;

namespace Chirp.Program;

public static class Program
{
    const string path = "../../data/chirp_cli_db.csv";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }

        //if (arguments == null) return;
        
        CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
        var cheeps = db.Read(4);
        app.MapGet("/read", () => cheeps);
        
        //TODO: make store cheerps
        
        app.Run();
    }

    private static void StoreCheeps(string message)
    {
        CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
        Cheep c = Cheep.NewCheep(message);
        db.Store(c);
        Console.WriteLine("Cheep posted successfully!");
    }
}
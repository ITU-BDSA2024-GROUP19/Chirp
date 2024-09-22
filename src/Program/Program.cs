using Chirp.SimpleDB;
using DocoptNet;
using Microsoft.AspNetCore.Builder;

namespace Chirp.Program;

public static class Program
{
    const string path = "../../data/chirp_cli_db.csv";

    public static void Main(string[] args)
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
            //UserInterface.PrintCheeps(db.Read(arguments["<limit>"].AsInt));
            var cheeps = db.Read(arguments["<limit>"].AsInt);
            app.MapGet("/cheeps", () => cheeps);
        }
        else if (arguments["cheep"].IsTrue)
        {
            StoreCheeps(arguments["<message>"].ToString());
        }
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
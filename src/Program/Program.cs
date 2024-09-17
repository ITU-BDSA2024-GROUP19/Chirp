﻿using Chirp.SimpleDB;
using DocoptNet;

namespace Chirp.Program;

public static class Program
{
    const string path = "../../data/chirp_cli_db.csv";

    public static void Main(string[] args)
    {
        var arguments = new Docopt().Apply(UserInterface.GetCLIMessage(), args, version: "0.1", exit: true);
        
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }

        if (arguments == null) return;
        
        if (arguments["read"].IsTrue)
        {
            CSVDatabase<Cheep> db = CSVDatabase<Cheep>.Instance(path);
            UserInterface.PrintCheeps(db.Read(arguments["<limit>"].AsInt));
        }
        else if (arguments["cheep"].IsTrue)
        {
            StoreCheeps(arguments["<message>"].ToString());
        }
    }

    private static void StoreCheeps(string message)
    {
        CSVDatabase<Cheep> db = CSVDatabase<Cheep>.Instance(path);
        Cheep c = Cheep.NewCheep(message);
        db.Store(c);
        Console.WriteLine("Cheep posted successfully!");
    }
}
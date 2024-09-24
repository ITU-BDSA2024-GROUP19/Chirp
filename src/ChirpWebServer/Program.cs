using Chirp.SimpleDB;
using Chirp.Program;

namespace ChirpWebServer
{
    public class Program
    {
        const string path = "../../data/chirp_cli_db.csv";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthorization();

            app.MapGet("/readCheeps", () =>
            {
                CsvDatabase<Cheep> db = CsvDatabase<Cheep>.Instance(path);
                var cheeps = db.Read(0);
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
}
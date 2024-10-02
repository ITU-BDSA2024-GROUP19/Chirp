using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Cheeps",
                columns: table => new
                {
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cheeps", x => x.Text);
                    table.ForeignKey(
                        name: "FK_Cheeps_Authors_AuthorName",
                        column: x => x.AuthorName,
                        principalTable: "Authors",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cheeps_AuthorName",
                table: "Cheeps",
                column: "AuthorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cheeps");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}

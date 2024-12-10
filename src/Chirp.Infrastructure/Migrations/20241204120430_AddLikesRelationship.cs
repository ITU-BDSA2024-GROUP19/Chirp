using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheepLikes",
                columns: table => new
                {
                    LikesCheepId = table.Column<int>(type: "INTEGER", nullable: false),
                    LikesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheepLikes", x => new { x.LikesCheepId, x.LikesId });
                    table.ForeignKey(
                        name: "FK_CheepLikes_AspNetUsers_LikesId",
                        column: x => x.LikesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheepLikes_Cheeps_LikesCheepId",
                        column: x => x.LikesCheepId,
                        principalTable: "Cheeps",
                        principalColumn: "CheepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheepLikes_LikesId",
                table: "CheepLikes",
                column: "LikesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheepLikes");
        }
    }
}

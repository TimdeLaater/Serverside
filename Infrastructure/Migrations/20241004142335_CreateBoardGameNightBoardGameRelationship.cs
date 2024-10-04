using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateBoardGameNightBoardGameRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGames_BoardGameNights_BoardGameNightId",
                table: "BoardGames");

            migrationBuilder.DropIndex(
                name: "IX_BoardGames_BoardGameNightId",
                table: "BoardGames");

            migrationBuilder.DropColumn(
                name: "BoardGameNightId",
                table: "BoardGames");

            migrationBuilder.CreateTable(
                name: "BoardGameBoardGameNight",
                columns: table => new
                {
                    BoardGameNightsBoardGameNightId = table.Column<int>(type: "int", nullable: false),
                    BoardGamesBoardGameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameBoardGameNight", x => new { x.BoardGameNightsBoardGameNightId, x.BoardGamesBoardGameId });
                    table.ForeignKey(
                        name: "FK_BoardGameBoardGameNight_BoardGameNights_BoardGameNightsBoardGameNightId",
                        column: x => x.BoardGameNightsBoardGameNightId,
                        principalTable: "BoardGameNights",
                        principalColumn: "BoardGameNightId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameBoardGameNight_BoardGames_BoardGamesBoardGameId",
                        column: x => x.BoardGamesBoardGameId,
                        principalTable: "BoardGames",
                        principalColumn: "BoardGameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameBoardGameNight_BoardGamesBoardGameId",
                table: "BoardGameBoardGameNight",
                column: "BoardGamesBoardGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGameBoardGameNight");

            migrationBuilder.AddColumn<int>(
                name: "BoardGameNightId",
                table: "BoardGames",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_BoardGameNightId",
                table: "BoardGames",
                column: "BoardGameNightId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGames_BoardGameNights_BoardGameNightId",
                table: "BoardGames",
                column: "BoardGameNightId",
                principalTable: "BoardGameNights",
                principalColumn: "BoardGameNightId");
        }
    }
}

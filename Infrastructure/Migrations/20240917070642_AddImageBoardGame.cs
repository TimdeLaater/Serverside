using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageBoardGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
    name: "Image",
    table: "BoardGames");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "BoardGames",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "BoardGames");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "BoardGames",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

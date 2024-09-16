using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMultipleCascadePathsForReviews2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Persons_ReviewId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Persons_ReviewId",
                table: "Reviews",
                column: "ReviewId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Persons_ReviewId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Persons_ReviewId",
                table: "Reviews",
                column: "ReviewId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

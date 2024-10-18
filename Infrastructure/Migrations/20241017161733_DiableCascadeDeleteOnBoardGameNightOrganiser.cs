using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiableCascadeDeleteOnBoardGameNightOrganiser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGameNights_Persons_OrganizerId",
                table: "BoardGameNights");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGameNights_Persons_OrganizerId",
                table: "BoardGameNights",
                column: "OrganizerId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGameNights_Persons_OrganizerId",
                table: "BoardGameNights");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGameNights_Persons_OrganizerId",
                table: "BoardGameNights",
                column: "OrganizerId",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

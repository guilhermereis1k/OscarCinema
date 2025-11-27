using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OscarCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsActiveDisabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SeatTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ExhibitionTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SeatTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ExhibitionTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}

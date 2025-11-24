using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OscarCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Users",
                type: "int",
                maxLength: 450,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApplicationUserId",
                table: "Users",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ApplicationUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Users");
        }
    }
}

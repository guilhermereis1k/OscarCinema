using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OscarCinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SessionCentralization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Movies_MovieId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Rooms_RoomId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_MovieId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_RoomId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CleaningTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TrailerTime",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Sessions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Sessions");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CleaningTime",
                table: "Sessions",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TrailerTime",
                table: "Sessions",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_MovieId",
                table: "Tickets",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RoomId",
                table: "Tickets",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Movies_MovieId",
                table: "Tickets",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Rooms_RoomId",
                table: "Tickets",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

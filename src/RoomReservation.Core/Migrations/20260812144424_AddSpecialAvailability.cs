using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingAvailabilities_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingSpecialAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingSpecialAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingSpecialAvailabilities_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomSpecialAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomSpecialAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomSpecialAvailabilities_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingAvailabilities_BuildingId",
                table: "BuildingAvailabilities",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingSpecialAvailabilities_BuildingId_StartDate_EndDate",
                table: "BuildingSpecialAvailabilities",
                columns: new[] { "BuildingId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomSpecialAvailabilities_RoomId_StartDate_EndDate",
                table: "RoomSpecialAvailabilities",
                columns: new[] { "RoomId", "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingAvailabilities");

            migrationBuilder.DropTable(
                name: "BuildingSpecialAvailabilities");

            migrationBuilder.DropTable(
                name: "RoomSpecialAvailabilities");
        }
    }
}

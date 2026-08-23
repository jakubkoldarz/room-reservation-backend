using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "Reservations",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "Reservations",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Reservations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CanceledById",
                table: "Reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Reservations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Reservations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedById",
                table: "Reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000000"), "reservation.view" },
                    { new Guid("50000000-0000-0000-0000-000000000001"), "reservation.list" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "reservation.force.cancel" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "reservation.approve" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "reservation.reject" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "reservation.create" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CanceledById",
                table: "Reservations",
                column: "CanceledById");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RejectedById",
                table: "Reservations",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId_Date",
                table: "Reservations",
                columns: new[] { "RoomId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_CanceledById",
                table: "Reservations",
                column: "CanceledById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_RejectedById",
                table: "Reservations",
                column: "RejectedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_CanceledById",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_RejectedById",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CanceledById",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RejectedById",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId_Date",
                table: "Reservations");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CanceledById",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "Reservations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Reservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");
        }
    }
}

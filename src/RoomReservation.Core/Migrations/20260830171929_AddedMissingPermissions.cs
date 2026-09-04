using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissingPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000000"), "role.view" },
                    { new Guid("70000000-0000-0000-0000-000000000001"), "role.list" },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "role.add" },
                    { new Guid("70000000-0000-0000-0000-000000000003"), "role.delete" },
                    { new Guid("70000000-0000-0000-0000-000000000004"), "role.edit" },
                    { new Guid("80000000-0000-0000-0000-000000000000"), "permission.list" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000000"));
        }
    }
}

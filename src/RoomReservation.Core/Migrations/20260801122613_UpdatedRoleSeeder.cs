using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRoleSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000002"), "user.block" });
        }
    }
}

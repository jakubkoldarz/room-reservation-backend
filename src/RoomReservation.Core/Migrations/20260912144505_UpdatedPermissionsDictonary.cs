using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPermissionsDictonary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("70000000-0000-0000-0000-000000000005"), "role.assign" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"));
        }
    }
}

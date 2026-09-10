using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMissingPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000000"), "building.view" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000000") },
                    { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000000") },
                    { new Guid("50000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000000") },
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("50000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000000") },
                    { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000000") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000000") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000000") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000000") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000000"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000005"), "room.edit.availability" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "building.edit.availability" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002") });
        }
    }
}

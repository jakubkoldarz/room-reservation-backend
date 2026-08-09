using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipments_Equipments_EquipmentId",
                table: "RoomEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipments_Rooms_RoomId",
                table: "RoomEquipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomEquipments",
                table: "RoomEquipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.RenameTable(
                name: "RoomEquipments",
                newName: "RoomEquipment");

            migrationBuilder.RenameTable(
                name: "Equipments",
                newName: "Equipment");

            migrationBuilder.RenameIndex(
                name: "IX_RoomEquipments_EquipmentId",
                table: "RoomEquipment",
                newName: "IX_RoomEquipment_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_Name",
                table: "Equipment",
                newName: "IX_Equipment_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomEquipment",
                table: "RoomEquipment",
                columns: new[] { "RoomId", "EquipmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000000"), "equipment.view" },
                    { new Guid("40000000-0000-0000-0000-000000000001"), "equipment.list" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "equipment.add" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "equipment.delete" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "equipment.edit" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipment_Equipment_EquipmentId",
                table: "RoomEquipment",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipment_Rooms_RoomId",
                table: "RoomEquipment",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipment_Equipment_EquipmentId",
                table: "RoomEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipment_Rooms_RoomId",
                table: "RoomEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomEquipment",
                table: "RoomEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.RenameTable(
                name: "RoomEquipment",
                newName: "RoomEquipments");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "Equipments");

            migrationBuilder.RenameIndex(
                name: "IX_RoomEquipment_EquipmentId",
                table: "RoomEquipments",
                newName: "IX_RoomEquipments_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_Name",
                table: "Equipments",
                newName: "IX_Equipments_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomEquipments",
                table: "RoomEquipments",
                columns: new[] { "RoomId", "EquipmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipments_Equipments_EquipmentId",
                table: "RoomEquipments",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipments_Rooms_RoomId",
                table: "RoomEquipments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

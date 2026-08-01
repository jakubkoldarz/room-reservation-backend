using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Room_RoomId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Users_ApprovedById",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Users_CreatedById",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Building_BuildingId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailability_Room_RoomId",
                table: "RoomAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipment_Equipment_EquipmentId",
                table: "RoomEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipment_Room_RoomId",
                table: "RoomEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomEquipment",
                table: "RoomEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAvailability",
                table: "RoomAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Building",
                table: "Building");

            migrationBuilder.RenameTable(
                name: "RoomEquipment",
                newName: "RoomEquipments");

            migrationBuilder.RenameTable(
                name: "RoomAvailability",
                newName: "RoomAvailabilities");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "Equipments");

            migrationBuilder.RenameTable(
                name: "Building",
                newName: "Buildings");

            migrationBuilder.RenameIndex(
                name: "IX_RoomEquipment_EquipmentId",
                table: "RoomEquipments",
                newName: "IX_RoomEquipments_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAvailability_RoomId",
                table: "RoomAvailabilities",
                newName: "IX_RoomAvailabilities_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Room_BuildingId_Identifier",
                table: "Rooms",
                newName: "IX_Rooms_BuildingId_Identifier");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_RoomId",
                table: "Reservations",
                newName: "IX_Reservations_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_CreatedById",
                table: "Reservations",
                newName: "IX_Reservations_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_ApprovedById",
                table: "Reservations",
                newName: "IX_Reservations_ApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_Name",
                table: "Equipments",
                newName: "IX_Equipments_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Building_Name",
                table: "Buildings",
                newName: "IX_Buildings_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomEquipments",
                table: "RoomEquipments",
                columns: new[] { "RoomId", "EquipmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAvailabilities",
                table: "RoomAvailabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_ApprovedById",
                table: "Reservations",
                column: "ApprovedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_CreatedById",
                table: "Reservations",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_RoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_ApprovedById",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_CreatedById",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipments_Equipments_EquipmentId",
                table: "RoomEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipments_Rooms_RoomId",
                table: "RoomEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomEquipments",
                table: "RoomEquipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAvailabilities",
                table: "RoomAvailabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "RoomEquipments",
                newName: "RoomEquipment");

            migrationBuilder.RenameTable(
                name: "RoomAvailabilities",
                newName: "RoomAvailability");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.RenameTable(
                name: "Equipments",
                newName: "Equipment");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "Building");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_BuildingId_Identifier",
                table: "Room",
                newName: "IX_Room_BuildingId_Identifier");

            migrationBuilder.RenameIndex(
                name: "IX_RoomEquipments_EquipmentId",
                table: "RoomEquipment",
                newName: "IX_RoomEquipment_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAvailabilities_RoomId",
                table: "RoomAvailability",
                newName: "IX_RoomAvailability_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservation",
                newName: "IX_Reservation_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CreatedById",
                table: "Reservation",
                newName: "IX_Reservation_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_ApprovedById",
                table: "Reservation",
                newName: "IX_Reservation_ApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_Name",
                table: "Equipment",
                newName: "IX_Equipment_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Buildings_Name",
                table: "Building",
                newName: "IX_Building_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomEquipment",
                table: "RoomEquipment",
                columns: new[] { "RoomId", "EquipmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAvailability",
                table: "RoomAvailability",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Building",
                table: "Building",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Room_RoomId",
                table: "Reservation",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Users_ApprovedById",
                table: "Reservation",
                column: "ApprovedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Users_CreatedById",
                table: "Reservation",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Building_BuildingId",
                table: "Room",
                column: "BuildingId",
                principalTable: "Building",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailability_Room_RoomId",
                table: "RoomAvailability",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipment_Equipment_EquipmentId",
                table: "RoomEquipment",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipment_Room_RoomId",
                table: "RoomEquipment",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

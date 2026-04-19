using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class IntCoord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9fad804a-c592-439b-89e6-e20aa74641e6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8b9486c-f338-48af-8a65-bb0d81822fec");

            migrationBuilder.AlterColumn<int>(
                name: "Location_Coordinates_Lng",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Location_Coordinates_Lat",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5edd9172-ddc9-4266-967b-dbc56df85656", null, "Admin", "ADMIN" },
                    { "d04188dc-7168-4942-80b8-df3b14d6b4bb", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5edd9172-ddc9-4266-967b-dbc56df85656");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d04188dc-7168-4942-80b8-df3b14d6b4bb");

            migrationBuilder.AlterColumn<double>(
                name: "Location_Coordinates_Lng",
                table: "Events",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Location_Coordinates_Lat",
                table: "Events",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9fad804a-c592-439b-89e6-e20aa74641e6", null, "User", "USER" },
                    { "e8b9486c-f338-48af-8a65-bb0d81822fec", null, "Admin", "ADMIN" }
                });
        }
    }
}

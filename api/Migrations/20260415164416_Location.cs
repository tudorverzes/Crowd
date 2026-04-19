using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07cbc58e-fad6-403d-80c1-345f9798c951");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f73d61fa-3d52-4169-a9c3-f0aea576ceb2");

            migrationBuilder.DropColumn(
                name: "Location_Coordinates_Lat",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Location_Coordinates_Lng",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "AdTarget");

            migrationBuilder.AddColumn<Point>(
                name: "Location_Geometry",
                table: "Events",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_AddressLine",
                table: "AdTarget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_City",
                table: "AdTarget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_Country",
                table: "AdTarget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location_Geometry",
                table: "AdTarget",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_StateOrRegion",
                table: "AdTarget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_VenueName",
                table: "AdTarget",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "59490eb5-6d39-414c-97f9-ce51e336145e", null, "User", "USER" },
                    { "906e7531-fef6-4672-8df3-084794b84257", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59490eb5-6d39-414c-97f9-ce51e336145e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "906e7531-fef6-4672-8df3-084794b84257");

            migrationBuilder.DropColumn(
                name: "Location_Geometry",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Location_AddressLine",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_City",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_Country",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_Geometry",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_StateOrRegion",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_VenueName",
                table: "AdTarget");

            migrationBuilder.AddColumn<int>(
                name: "Location_Coordinates_Lat",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Location_Coordinates_Lng",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "AdTarget",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "AdTarget",
                type: "float",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07cbc58e-fad6-403d-80c1-345f9798c951", null, "Admin", "ADMIN" },
                    { "f73d61fa-3d52-4169-a9c3-f0aea576ceb2", null, "User", "USER" }
                });
        }
    }
}

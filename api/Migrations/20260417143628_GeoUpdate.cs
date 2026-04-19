using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class GeoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28a5d883-d1b5-468a-9d1c-27fb27e84eaf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83c1a2a0-f9a1-489c-898a-fb15d2771f59");

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
                name: "Location_StateOrRegion",
                table: "AdTarget");

            migrationBuilder.DropColumn(
                name: "Location_VenueName",
                table: "AdTarget");

            migrationBuilder.RenameColumn(
                name: "Location_Geometry",
                table: "AdTarget",
                newName: "GeoRadiusTarget_Geometry");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "059d9698-2d74-4df8-8ccd-beb0036317ed", null, "User", "USER" },
                    { "49449419-b3d5-4f8a-9259-0f098d6cc524", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "059d9698-2d74-4df8-8ccd-beb0036317ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49449419-b3d5-4f8a-9259-0f098d6cc524");

            migrationBuilder.RenameColumn(
                name: "GeoRadiusTarget_Geometry",
                table: "AdTarget",
                newName: "Location_Geometry");

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
                    { "28a5d883-d1b5-468a-9d1c-27fb27e84eaf", null, "User", "USER" },
                    { "83c1a2a0-f9a1-489c-898a-fb15d2771f59", null, "Admin", "ADMIN" }
                });
        }
    }
}

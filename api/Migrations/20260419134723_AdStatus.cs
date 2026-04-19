using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AdStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "059d9698-2d74-4df8-8ccd-beb0036317ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49449419-b3d5-4f8a-9259-0f098d6cc524");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Ads");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4da1f45f-0bbf-4777-a6e7-93ba0a93568e", null, "Admin", "ADMIN" },
                    { "e176e0cc-106d-45af-8108-21bd515a22e3", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4da1f45f-0bbf-4777-a6e7-93ba0a93568e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e176e0cc-106d-45af-8108-21bd515a22e3");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Ads");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "059d9698-2d74-4df8-8ccd-beb0036317ed", null, "User", "USER" },
                    { "49449419-b3d5-4f8a-9259-0f098d6cc524", null, "Admin", "ADMIN" }
                });
        }
    }
}

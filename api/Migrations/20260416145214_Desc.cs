using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Desc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f6212a8-7dd2-4b0d-9ddf-a0ebe6daa875");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9be2ec2b-4e59-4646-a0dd-822783337753");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "577ee71f-af64-49da-9e23-878c4ba8fa6e", null, "Admin", "ADMIN" },
                    { "bbdd9998-08d9-4715-bcb1-4bd945fdbdc5", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "577ee71f-af64-49da-9e23-878c4ba8fa6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbdd9998-08d9-4715-bcb1-4bd945fdbdc5");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Events");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f6212a8-7dd2-4b0d-9ddf-a0ebe6daa875", null, "User", "USER" },
                    { "9be2ec2b-4e59-4646-a0dd-822783337753", null, "Admin", "ADMIN" }
                });
        }
    }
}

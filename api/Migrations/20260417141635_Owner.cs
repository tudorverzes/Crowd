using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Owner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "577ee71f-af64-49da-9e23-878c4ba8fa6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbdd9998-08d9-4715-bcb1-4bd945fdbdc5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "28a5d883-d1b5-468a-9d1c-27fb27e84eaf", null, "User", "USER" },
                    { "83c1a2a0-f9a1-489c-898a-fb15d2771f59", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28a5d883-d1b5-468a-9d1c-27fb27e84eaf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83c1a2a0-f9a1-489c-898a-fb15d2771f59");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "577ee71f-af64-49da-9e23-878c4ba8fa6e", null, "Admin", "ADMIN" },
                    { "bbdd9998-08d9-4715-bcb1-4bd945fdbdc5", null, "User", "USER" }
                });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class TargetAdds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5edd9172-ddc9-4266-967b-dbc56df85656");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d04188dc-7168-4942-80b8-df3b14d6b4bb");

            migrationBuilder.AddColumn<bool>(
                name: "VisibleForTargetedAds",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "65391c4d-79f3-48a9-be50-d9be11760907", null, "User", "USER" },
                    { "fb84dbb6-662a-447d-a2db-a28adf3e7e0b", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65391c4d-79f3-48a9-be50-d9be11760907");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fb84dbb6-662a-447d-a2db-a28adf3e7e0b");

            migrationBuilder.DropColumn(
                name: "VisibleForTargetedAds",
                table: "Events");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5edd9172-ddc9-4266-967b-dbc56df85656", null, "Admin", "ADMIN" },
                    { "d04188dc-7168-4942-80b8-df3b14d6b4bb", null, "User", "USER" }
                });
        }
    }
}

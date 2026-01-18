using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saistones.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DisplayName", "Email" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Ganesh", "ganesh@test.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}

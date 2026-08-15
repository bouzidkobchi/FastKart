using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastKart.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Client",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 13, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Client",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 13, 8, 4, 19, 585, DateTimeKind.Utc).AddTicks(8249));
        }
    }
}

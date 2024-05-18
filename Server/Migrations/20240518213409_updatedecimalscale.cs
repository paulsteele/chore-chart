using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hub.Server.Migrations
{
    /// <inheritdoc />
    public partial class updatedecimalscale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Categories",
                type: "decimal(10,0)",
                precision: 10,
                scale: 0,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Categories",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)",
                oldPrecision: 10,
                oldScale: 0);
        }
    }
}

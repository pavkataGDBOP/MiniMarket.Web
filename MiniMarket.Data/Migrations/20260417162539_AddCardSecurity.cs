using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCardSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardHash",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardLast4",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardHash",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CardLast4",
                table: "Orders");
        }
    }
}

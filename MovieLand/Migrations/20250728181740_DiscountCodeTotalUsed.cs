using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieLand.Migrations
{
    /// <inheritdoc />
    public partial class DiscountCodeTotalUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalUsed",
                table: "DiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalUsed",
                table: "DiscountCodes");
        }
    }
}

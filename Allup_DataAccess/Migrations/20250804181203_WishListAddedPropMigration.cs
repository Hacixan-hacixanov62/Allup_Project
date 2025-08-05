using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allup_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class WishListAddedPropMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "WishlistItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "WishlistItems");
        }
    }
}

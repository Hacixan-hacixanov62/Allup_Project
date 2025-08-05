using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allup_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProductTableUrlMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainFileImage",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainFileUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainFileImage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MainFileUrl",
                table: "Products");
        }
    }
}

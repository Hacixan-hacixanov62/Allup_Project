using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allup_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BlogCommentRemovePropMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "BlogComments");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlogComments");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "BlogComments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BlogComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlogComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "BlogComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

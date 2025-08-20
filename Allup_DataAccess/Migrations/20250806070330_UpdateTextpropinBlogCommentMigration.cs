using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allup_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTextpropinBlogCommentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "BlogComments",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "BlogComments",
                newName: "Text");
        }
    }
}

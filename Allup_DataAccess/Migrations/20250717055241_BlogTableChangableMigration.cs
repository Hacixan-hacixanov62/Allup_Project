using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allup_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BlogTableChangableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_TagId",
                table: "Blogs",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Tags_TagId",
                table: "Blogs",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Tags_TagId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_TagId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Blogs");
        }
    }
}

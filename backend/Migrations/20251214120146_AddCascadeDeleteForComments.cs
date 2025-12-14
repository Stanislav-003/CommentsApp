using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_comment_parent_id",
                table: "comment");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_comment_parent_id",
                table: "comment",
                column: "parent_id",
                principalTable: "comment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_comment_parent_id",
                table: "comment");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_comment_parent_id",
                table: "comment",
                column: "parent_id",
                principalTable: "comment",
                principalColumn: "id");
        }
    }
}

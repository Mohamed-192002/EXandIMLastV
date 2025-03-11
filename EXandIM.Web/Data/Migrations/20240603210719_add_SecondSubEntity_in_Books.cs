using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_SecondSubEntity_in_Books : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondSubEntityId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_SecondSubEntityId",
                table: "Books",
                column: "SecondSubEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_SecondSubEntities_SecondSubEntityId",
                table: "Books",
                column: "SecondSubEntityId",
                principalTable: "SecondSubEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_SecondSubEntities_SecondSubEntityId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_SecondSubEntityId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SecondSubEntityId",
                table: "Books");
        }
    }
}

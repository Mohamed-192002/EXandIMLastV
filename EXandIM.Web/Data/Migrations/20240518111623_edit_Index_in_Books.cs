using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_Index_in_Books : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Title_BookDate",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_BookDate_IsExport",
                table: "Books",
                columns: new[] { "Title", "BookDate", "IsExport" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Title_BookDate_IsExport",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_BookDate",
                table: "Books",
                columns: new[] { "Title", "BookDate" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_ReferenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "ReferenceNumbers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "ReferenceNumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_Hidden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Readings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Activities");
        }
    }
}

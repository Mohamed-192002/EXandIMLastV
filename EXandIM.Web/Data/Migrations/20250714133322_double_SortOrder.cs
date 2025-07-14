using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class double_SortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SortOrder",
                table: "ItemsInActivity",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "ItemsInActivity",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}

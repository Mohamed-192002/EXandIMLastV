using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_circle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CircleId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Circles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CircleId",
                table: "Teams",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_Circles_Name",
                table: "Circles",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Circles_CircleId",
                table: "Teams",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Circles_CircleId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Circles");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CircleId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CircleId",
                table: "Teams");
        }
    }
}

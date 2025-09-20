using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_ReferenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceNumberId",
                table: "Readings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceNumberId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReferenceNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceNumbers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Readings_ReferenceNumberId",
                table: "Readings",
                column: "ReferenceNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ReferenceNumberId",
                table: "Books",
                column: "ReferenceNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceNumbers_Name",
                table: "ReferenceNumbers",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_ReferenceNumbers_ReferenceNumberId",
                table: "Books",
                column: "ReferenceNumberId",
                principalTable: "ReferenceNumbers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_ReferenceNumbers_ReferenceNumberId",
                table: "Readings",
                column: "ReferenceNumberId",
                principalTable: "ReferenceNumbers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_ReferenceNumbers_ReferenceNumberId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Readings_ReferenceNumbers_ReferenceNumberId",
                table: "Readings");

            migrationBuilder.DropTable(
                name: "ReferenceNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Readings_ReferenceNumberId",
                table: "Readings");

            migrationBuilder.DropIndex(
                name: "IX_Books_ReferenceNumberId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReferenceNumberId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "ReferenceNumberId",
                table: "Books");
        }
    }
}

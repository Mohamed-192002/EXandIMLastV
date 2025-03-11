using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemsInActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    ReadingId = table.Column<int>(type: "int", nullable: true),
                    Procedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityBookId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsInActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsInActivity_Activities_ActivityBookId",
                        column: x => x.ActivityBookId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsInActivity_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemsInActivity_Readings_ReadingId",
                        column: x => x.ReadingId,
                        principalTable: "Readings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsInActivity_ActivityBookId",
                table: "ItemsInActivity",
                column: "ActivityBookId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsInActivity_BookId",
                table: "ItemsInActivity",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsInActivity_ReadingId",
                table: "ItemsInActivity",
                column: "ReadingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsInActivity");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class delete_teams_fromReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingSubEntity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadingSubEntity",
                columns: table => new
                {
                    ReadingsId = table.Column<int>(type: "int", nullable: false),
                    TeamsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingSubEntity", x => new { x.ReadingsId, x.TeamsId });
                    table.ForeignKey(
                        name: "FK_ReadingSubEntity_Readings_ReadingsId",
                        column: x => x.ReadingsId,
                        principalTable: "Readings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReadingSubEntity_SubEntities_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "SubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingSubEntity_TeamsId",
                table: "ReadingSubEntity",
                column: "TeamsId");
        }
    }
}

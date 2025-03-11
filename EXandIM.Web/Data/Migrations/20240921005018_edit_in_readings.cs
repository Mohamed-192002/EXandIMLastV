using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_in_readings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_Entities_EntityId",
                table: "Readings");

            migrationBuilder.DropIndex(
                name: "IX_Readings_EntityId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Readings");

            migrationBuilder.CreateTable(
                name: "EntityReading",
                columns: table => new
                {
                    EntitiesId = table.Column<int>(type: "int", nullable: false),
                    ReadingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityReading", x => new { x.EntitiesId, x.ReadingsId });
                    table.ForeignKey(
                        name: "FK_EntityReading_Entities_EntitiesId",
                        column: x => x.EntitiesId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityReading_Readings_ReadingsId",
                        column: x => x.ReadingsId,
                        principalTable: "Readings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingSecondSubEntity",
                columns: table => new
                {
                    ReadingsId = table.Column<int>(type: "int", nullable: false),
                    SecondSubEntitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingSecondSubEntity", x => new { x.ReadingsId, x.SecondSubEntitiesId });
                    table.ForeignKey(
                        name: "FK_ReadingSecondSubEntity_Readings_ReadingsId",
                        column: x => x.ReadingsId,
                        principalTable: "Readings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadingSecondSubEntity_SecondSubEntities_SecondSubEntitiesId",
                        column: x => x.SecondSubEntitiesId,
                        principalTable: "SecondSubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReadingSubEntity",
                columns: table => new
                {
                    ReadingsId = table.Column<int>(type: "int", nullable: false),
                    SubEntitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingSubEntity", x => new { x.ReadingsId, x.SubEntitiesId });
                    table.ForeignKey(
                        name: "FK_ReadingSubEntity_Readings_ReadingsId",
                        column: x => x.ReadingsId,
                        principalTable: "Readings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadingSubEntity_SubEntities_SubEntitiesId",
                        column: x => x.SubEntitiesId,
                        principalTable: "SubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityReading_ReadingsId",
                table: "EntityReading",
                column: "ReadingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingSecondSubEntity_SecondSubEntitiesId",
                table: "ReadingSecondSubEntity",
                column: "SecondSubEntitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingSubEntity_SubEntitiesId",
                table: "ReadingSubEntity",
                column: "SubEntitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityReading");

            migrationBuilder.DropTable(
                name: "ReadingSecondSubEntity");

            migrationBuilder.DropTable(
                name: "ReadingSubEntity");

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Readings_EntityId",
                table: "Readings",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_Entities_EntityId",
                table: "Readings",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

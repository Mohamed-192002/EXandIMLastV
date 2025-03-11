using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_SecondSubEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecondSubEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    SubEntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondSubEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecondSubEntities_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecondSubEntities_Entities_SubEntityId",
                        column: x => x.SubEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecondSubEntities_EntityId",
                table: "SecondSubEntities",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SecondSubEntities_Name_EntityId_SubEntityId",
                table: "SecondSubEntities",
                columns: new[] { "Name", "EntityId", "SubEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecondSubEntities_SubEntityId",
                table: "SecondSubEntities",
                column: "SubEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecondSubEntities");
        }
    }
}

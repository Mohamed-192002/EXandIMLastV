using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_SideEntities_SideEntityId",
                table: "Readings");

            migrationBuilder.RenameColumn(
                name: "SideEntityId",
                table: "Readings",
                newName: "EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Readings_SideEntityId",
                table: "Readings",
                newName: "IX_Readings_EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_Entities_EntityId",
                table: "Readings",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_Entities_EntityId",
                table: "Readings");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Readings",
                newName: "SideEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Readings_EntityId",
                table: "Readings",
                newName: "IX_Readings_SideEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_SideEntities_SideEntityId",
                table: "Readings",
                column: "SideEntityId",
                principalTable: "SideEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

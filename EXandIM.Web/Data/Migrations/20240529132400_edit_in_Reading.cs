using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_in_Reading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_Entities_EntityId",
                table: "Readings");

            migrationBuilder.DropForeignKey(
                name: "FK_Readings_SubEntities_SubEntityId",
                table: "Readings");

            migrationBuilder.DropIndex(
                name: "IX_Readings_EntityId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Readings");

            migrationBuilder.RenameColumn(
                name: "SubEntityId",
                table: "Readings",
                newName: "SideEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Readings_SubEntityId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Readings_SideEntities_SideEntityId",
                table: "Readings");

            migrationBuilder.RenameColumn(
                name: "SideEntityId",
                table: "Readings",
                newName: "SubEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Readings_SideEntityId",
                table: "Readings",
                newName: "IX_Readings_SubEntityId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Readings_SubEntities_SubEntityId",
                table: "Readings",
                column: "SubEntityId",
                principalTable: "SubEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

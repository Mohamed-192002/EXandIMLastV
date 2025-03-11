using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addtional_in_Readings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubEntityId",
                table: "Readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Readings_EntityId",
                table: "Readings",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Readings_SubEntityId",
                table: "Readings",
                column: "SubEntityId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Readings_SubEntityId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Readings");

            migrationBuilder.DropColumn(
                name: "SubEntityId",
                table: "Readings");
        }
    }
}

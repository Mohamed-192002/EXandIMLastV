using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_Circle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInside",
                table: "SubEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Circles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Circles_EntityId",
                table: "Circles",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circles_Entities_EntityId",
                table: "Circles",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circles_Entities_EntityId",
                table: "Circles");

            migrationBuilder.DropIndex(
                name: "IX_Circles_EntityId",
                table: "Circles");

            migrationBuilder.DropColumn(
                name: "IsInside",
                table: "SubEntities");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Circles");
        }
    }
}

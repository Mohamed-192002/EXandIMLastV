using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_team : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SideEntityId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsInside",
                table: "SecondSubEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SideEntityId",
                table: "Teams",
                column: "SideEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_SideEntities_SideEntityId",
                table: "Teams",
                column: "SideEntityId",
                principalTable: "SideEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_SideEntities_SideEntityId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SideEntityId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SideEntityId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsInside",
                table: "SecondSubEntities");
        }
    }
}

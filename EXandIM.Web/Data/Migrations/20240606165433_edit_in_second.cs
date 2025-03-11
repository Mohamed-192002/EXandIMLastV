using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_in_second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SecondSubEntities_Entities_SubEntityId",
                table: "SecondSubEntities");

            migrationBuilder.AddForeignKey(
                name: "FK_SecondSubEntities_SubEntities_SubEntityId",
                table: "SecondSubEntities",
                column: "SubEntityId",
                principalTable: "SubEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SecondSubEntities_SubEntities_SubEntityId",
                table: "SecondSubEntities");

            migrationBuilder.AddForeignKey(
                name: "FK_SecondSubEntities_Entities_SubEntityId",
                table: "SecondSubEntities",
                column: "SubEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

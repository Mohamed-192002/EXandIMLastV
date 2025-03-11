using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Books_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    SubEntityId = table.Column<int>(type: "int", nullable: false),
                    SideEntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Books_SideEntities_SideEntityId",
                        column: x => x.SideEntityId,
                        principalTable: "SideEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Books_SubEntities_SubEntityId",
                        column: x => x.SubEntityId,
                        principalTable: "SubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_EntityId",
                table: "Books",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SideEntityId",
                table: "Books",
                column: "SideEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SubEntityId",
                table: "Books",
                column: "SubEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_BookDate",
                table: "Books",
                columns: new[] { "Title", "BookDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}

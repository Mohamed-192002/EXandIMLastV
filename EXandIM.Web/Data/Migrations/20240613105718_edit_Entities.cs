using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Entities_EntityId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_SecondSubEntities_SecondSubEntityId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_SubEntities_SubEntityId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_EntityId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_SecondSubEntityId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_SubEntityId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SecondSubEntityId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SubEntityId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookEntity",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    EntitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookEntity", x => new { x.BooksId, x.EntitiesId });
                    table.ForeignKey(
                        name: "FK_BookEntity_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookEntity_Entities_EntitiesId",
                        column: x => x.EntitiesId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookSecondSubEntity",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    SecondSubEntitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSecondSubEntity", x => new { x.BooksId, x.SecondSubEntitiesId });
                    table.ForeignKey(
                        name: "FK_BookSecondSubEntity_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookSecondSubEntity_SecondSubEntities_SecondSubEntitiesId",
                        column: x => x.SecondSubEntitiesId,
                        principalTable: "SecondSubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookSubEntity",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    SubEntitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSubEntity", x => new { x.BooksId, x.SubEntitiesId });
                    table.ForeignKey(
                        name: "FK_BookSubEntity_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookSubEntity_SubEntities_SubEntitiesId",
                        column: x => x.SubEntitiesId,
                        principalTable: "SubEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookEntity_EntitiesId",
                table: "BookEntity",
                column: "EntitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_BookSecondSubEntity_SecondSubEntitiesId",
                table: "BookSecondSubEntity",
                column: "SecondSubEntitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_BookSubEntity_SubEntitiesId",
                table: "BookSubEntity",
                column: "SubEntitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookEntity");

            migrationBuilder.DropTable(
                name: "BookSecondSubEntity");

            migrationBuilder.DropTable(
                name: "BookSubEntity");

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondSubEntityId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubEntityId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_EntityId",
                table: "Books",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SecondSubEntityId",
                table: "Books",
                column: "SecondSubEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SubEntityId",
                table: "Books",
                column: "SubEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Entities_EntityId",
                table: "Books",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_SecondSubEntities_SecondSubEntityId",
                table: "Books",
                column: "SecondSubEntityId",
                principalTable: "SecondSubEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_SubEntities_SubEntityId",
                table: "Books",
                column: "SubEntityId",
                principalTable: "SubEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

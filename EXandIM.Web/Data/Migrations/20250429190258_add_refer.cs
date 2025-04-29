using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXandIM.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_refer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReferTo",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReferDate",
                table: "Books",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferToBookNumber",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReferTo",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReferDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReferToBookNumber",
                table: "Books");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class Refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Election_Users_UserId",
                table: "Election");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Election",
                table: "Election");

            migrationBuilder.RenameTable(
                name: "Election",
                newName: "Elections");

            migrationBuilder.RenameIndex(
                name: "IX_Election_UserId",
                table: "Elections",
                newName: "IX_Elections_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Elections",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Elections",
                table: "Elections",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Elections_Users_UserId",
                table: "Elections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elections_Users_UserId",
                table: "Elections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Elections",
                table: "Elections");

            migrationBuilder.RenameTable(
                name: "Elections",
                newName: "Election");

            migrationBuilder.RenameIndex(
                name: "IX_Elections_UserId",
                table: "Election",
                newName: "IX_Election_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Election",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Election",
                table: "Election",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Election_Users_UserId",
                table: "Election",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

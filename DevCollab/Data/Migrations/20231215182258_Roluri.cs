using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevCollab.Data.Migrations
{
    public partial class Roluri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_ApplicationUserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AspNetUsers_ApplicationUserId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Subjects",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_ApplicationUserId",
                table: "Subjects",
                newName: "IX_Subjects_UserId");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Answers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_ApplicationUserId",
                table: "Answers",
                newName: "IX_Answers_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_AspNetUsers_UserId",
                table: "Subjects",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AspNetUsers_UserId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Subjects",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_UserId",
                table: "Subjects",
                newName: "IX_Subjects_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Answers",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                newName: "IX_Answers_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_ApplicationUserId",
                table: "Answers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_AspNetUsers_ApplicationUserId",
                table: "Subjects",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

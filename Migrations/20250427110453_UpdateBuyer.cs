using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBuyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Buyers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buyers_UserId",
                table: "Buyers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buyers_User_UserId",
                table: "Buyers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buyers_User_UserId",
                table: "Buyers");

            migrationBuilder.DropIndex(
                name: "IX_Buyers_UserId",
                table: "Buyers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Buyers");
        }
    }
}

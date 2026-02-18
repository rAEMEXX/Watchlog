using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchLog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTitleProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    CurrentSeason = table.Column<int>(type: "int", nullable: true),
                    CurrentEpisode = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTitleProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTitleProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTitleProgresses_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTitleProgresses_TitleId",
                table: "UserTitleProgresses",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTitleProgresses_UserId",
                table: "UserTitleProgresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTitleProgresses");
        }
    }
}

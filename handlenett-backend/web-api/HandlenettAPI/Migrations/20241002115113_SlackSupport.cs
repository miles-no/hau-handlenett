using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandlenettAPI.Migrations
{
    /// <inheritdoc />
    public partial class SlackSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlackUserId",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlackUserId",
                table: "Users");
        }
    }
}

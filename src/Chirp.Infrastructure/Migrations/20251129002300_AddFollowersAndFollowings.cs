using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowersAndFollowings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "followers",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "followings",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "followers",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "followings",
                table: "Authors");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeCountToTopicAndReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Replies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Replies");
        }
    }
}

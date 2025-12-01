using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E1.Backend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImageAndArticleToGuides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArticleUrl",
                table: "RecyclingGuides",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RecyclingGuides",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleUrl",
                table: "RecyclingGuides");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RecyclingGuides");
        }
    }
}

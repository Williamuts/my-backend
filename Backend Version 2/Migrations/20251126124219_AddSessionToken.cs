using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E1.Backend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentSessionToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSessionToken",
                table: "AspNetUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekthantering.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCardStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Cards",
                type: "TEXT",
                nullable: false,
                defaultValue: "todo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cards");
        }
    }
}

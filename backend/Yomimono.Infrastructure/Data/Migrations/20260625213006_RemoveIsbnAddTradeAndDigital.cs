using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yomimono.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsbnAddTradeAndDigital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Isbn",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "Books");

            migrationBuilder.AddColumn<bool>(
                name: "IsDigital",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTradePaperback",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TradeEdition",
                table: "Books",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDigital",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsTradePaperback",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TradeEdition",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                column: "Isbn",
                unique: true);
        }
    }
}

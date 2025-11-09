using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiCore.Migrations
{
    /// <inheritdoc />
    public partial class PaymentFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "PaymentLinks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "PaymentLinks");
        }
    }
}

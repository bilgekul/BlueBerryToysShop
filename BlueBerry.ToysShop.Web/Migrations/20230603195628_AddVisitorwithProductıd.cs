using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBerry.ToysShop.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorwithProductıd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Visitors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Visitors");
        }
    }
}

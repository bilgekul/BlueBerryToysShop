using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBerry.ToysShop.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProductRating",
                table: "Visitors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductRating",
                table: "Visitors");
        }
    }
}

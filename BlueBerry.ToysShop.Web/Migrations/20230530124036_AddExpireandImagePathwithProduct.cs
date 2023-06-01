using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueBerry.ToysShop.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddExpireandImagePathwithProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Expire",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expire",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Products");
        }
    }
}

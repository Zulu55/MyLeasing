using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLeasing.Web.Migrations
{
    public partial class AddLatLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "PropertyImages",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Properties",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Properties",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "PropertyImages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

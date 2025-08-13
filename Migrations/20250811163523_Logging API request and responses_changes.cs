using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfMe.Migrations
{
    /// <inheritdoc />
    public partial class LoggingAPIrequestandresponses_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Tbl_APILogger",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Tbl_APILogger",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "Tbl_APILogger",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "Tbl_APILogger");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Tbl_APILogger");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "Tbl_APILogger");
        }
    }
}

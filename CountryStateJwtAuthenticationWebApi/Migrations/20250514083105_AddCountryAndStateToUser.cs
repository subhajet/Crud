using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CountryStateJwtAuthenticationWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndStateToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "AspNetCoreRegistration_Subhajit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateName",
                table: "AspNetCoreRegistration_Subhajit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "AspNetCoreRegistration_Subhajit");

            migrationBuilder.DropColumn(
                name: "StateName",
                table: "AspNetCoreRegistration_Subhajit");
        }
    }
}

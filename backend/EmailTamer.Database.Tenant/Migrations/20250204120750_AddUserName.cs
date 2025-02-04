using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class AddUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "EmailBoxes",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.Sql("UPDATE EmailBoxes SET UserName = Name");
            
            migrationBuilder.DropColumn(
                name: "Name",
                table: "EmailBoxes");

            migrationBuilder.AddColumn<bool>(
                name: "AuthenticateByEmail",
                table: "EmailBoxes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BoxName",
                table: "EmailBoxes",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticateByEmail",
                table: "EmailBoxes");

            migrationBuilder.DropColumn(
                name: "BoxName",
                table: "EmailBoxes");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EmailBoxes",
                type: "longtext",
                nullable: true);

            migrationBuilder.Sql("UPDATE EmailBoxes SET Name = UserName");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "EmailBoxes");
        }
    }
}

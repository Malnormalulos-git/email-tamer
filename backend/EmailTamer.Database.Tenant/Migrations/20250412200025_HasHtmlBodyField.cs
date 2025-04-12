using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class HasHtmlBodyField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasHtmlBody",
                table: "Messages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasHtmlBody",
                table: "Messages");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentFilesNamesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFilesNames",
                table: "Messages",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFilesNames",
                table: "Messages");
        }
    }
}

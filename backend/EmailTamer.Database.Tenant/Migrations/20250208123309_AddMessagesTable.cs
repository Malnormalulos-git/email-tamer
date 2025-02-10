using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    EmailBoxId = table.Column<Guid>(type: "char(36)", nullable: false),
                    InReplyTo = table.Column<string>(type: "longtext", nullable: true),
                    Subject = table.Column<string>(type: "longtext", nullable: true),
                    TextBody = table.Column<string>(type: "longtext", nullable: true),
                    References = table.Column<string>(type: "longtext", nullable: false),
                    From = table.Column<string>(type: "longtext", nullable: false),
                    To = table.Column<string>(type: "longtext", nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME(0)", precision: 0, nullable: false),
                    ResentDate = table.Column<DateTime>(type: "DATETIME(0)", precision: 0, nullable: true),
                    S3FolderName = table.Column<string>(type: "longtext", nullable: false),
                    Folders = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_EmailBoxes_EmailBoxId",
                        column: x => x.EmailBoxId,
                        principalTable: "EmailBoxes",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EmailBoxId",
                table: "Messages",
                column: "EmailBoxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}

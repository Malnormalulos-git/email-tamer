using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class ManyMessagesToManyEmailBoxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_EmailBoxes_EmailBoxId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_EmailBoxId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EmailBoxId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "S3FolderName",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "EmailBoxMessage",
                columns: table => new
                {
                    EmailBoxesId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MessagesId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBoxMessage", x => new { x.EmailBoxesId, x.MessagesId });
                    table.ForeignKey(
                        name: "FK_EmailBoxMessage_EmailBoxes_EmailBoxesId",
                        column: x => x.EmailBoxesId,
                        principalTable: "EmailBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailBoxMessage_Messages_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmailBoxMessage_MessagesId",
                table: "EmailBoxMessage",
                column: "MessagesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailBoxMessage");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailBoxId",
                table: "Messages",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "S3FolderName",
                table: "Messages",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EmailBoxId",
                table: "Messages",
                column: "EmailBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_EmailBoxes_EmailBoxId",
                table: "Messages",
                column: "EmailBoxId",
                principalTable: "EmailBoxes",
                principalColumn: "Id");
        }
    }
}

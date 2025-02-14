using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class AddFoldersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Folders",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.CheckConstraint("CHK_Folders_IdNotDefault", "\"Id\" <> '00000000-0000-0000-0000-000000000000'");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FolderMessage",
                columns: table => new
                {
                    FoldersId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MessagesId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderMessage", x => new { x.FoldersId, x.MessagesId });
                    table.ForeignKey(
                        name: "FK_FolderMessage_Folders_FoldersId",
                        column: x => x.FoldersId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderMessage_Messages_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FolderMessage_MessagesId",
                table: "FolderMessage",
                column: "MessagesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderMessage");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.AddColumn<string>(
                name: "Folders",
                table: "Messages",
                type: "longtext",
                nullable: false);
        }
    }
}

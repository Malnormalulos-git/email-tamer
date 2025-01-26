using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmailBoxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false),
                    EmailDomainConnectionHost = table.Column<string>(type: "longtext", nullable: false),
                    EmailDomainConnectionPort = table.Column<int>(type: "int", nullable: false),
                    UseSSl = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "DATETIME(0)", precision: 0, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME(0)", precision: 0, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "DATETIME(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBoxes", x => x.Id);
                    table.CheckConstraint("CHK_EmailBoxes_IdNotDefault", "\"Id\" <> '00000000-0000-0000-0000-000000000000'");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmailBoxes_CreatedAt_ModifiedAt",
                table: "EmailBoxes",
                columns: new[] { "CreatedAt", "ModifiedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailBoxes");
        }
    }
}

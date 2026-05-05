using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SenderProfilesAndTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    SubjectTemplate = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BodyTemplate = table.Column<string>(type: "TEXT", maxLength: 20000, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SenderProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrganizationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Signature = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SenderProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_IsActive",
                table: "EmailTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_Name",
                table: "EmailTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SenderProfiles_IsActive",
                table: "SenderProfiles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SenderProfiles_IsDefault",
                table: "SenderProfiles",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_SenderProfiles_NormalizedEmail",
                table: "SenderProfiles",
                column: "NormalizedEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "SenderProfiles");
        }
    }
}

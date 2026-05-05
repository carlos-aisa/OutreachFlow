using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentAssetsAndTemplateAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachmentAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateAttachments",
                columns: table => new
                {
                    EmailTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttachmentAssetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateAttachments", x => new { x.EmailTemplateId, x.AttachmentAssetId });
                    table.ForeignKey(
                        name: "FK_EmailTemplateAttachments_AttachmentAssets_AttachmentAssetId",
                        column: x => x.AttachmentAssetId,
                        principalTable: "AttachmentAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailTemplateAttachments_EmailTemplates_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAssets_FileName",
                table: "AttachmentAssets",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAssets_IsActive",
                table: "AttachmentAssets",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAssets_Name",
                table: "AttachmentAssets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateAttachments_AttachmentAssetId",
                table: "EmailTemplateAttachments",
                column: "AttachmentAssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplateAttachments");

            migrationBuilder.DropTable(
                name: "AttachmentAssets");
        }
    }
}

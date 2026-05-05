using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailDraftGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SenderProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 20000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    HasRenderErrors = table.Column<bool>(type: "INTEGER", nullable: false),
                    MissingVariablesJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    UnknownVariablesJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailDrafts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailDrafts_EmailTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EmailDrafts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EmailDrafts_SenderProfiles_SenderProfileId",
                        column: x => x.SenderProfileId,
                        principalTable: "SenderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailDraftAttachments",
                columns: table => new
                {
                    EmailDraftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttachmentAssetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailDraftAttachments", x => new { x.EmailDraftId, x.AttachmentAssetId });
                    table.ForeignKey(
                        name: "FK_EmailDraftAttachments_AttachmentAssets_AttachmentAssetId",
                        column: x => x.AttachmentAssetId,
                        principalTable: "AttachmentAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailDraftAttachments_EmailDrafts_EmailDraftId",
                        column: x => x.EmailDraftId,
                        principalTable: "EmailDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailDraftAttachments_AttachmentAssetId",
                table: "EmailDraftAttachments",
                column: "AttachmentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_ContactId",
                table: "EmailDrafts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_CreatedAt",
                table: "EmailDrafts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_OrganizationId",
                table: "EmailDrafts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_SenderProfileId",
                table: "EmailDrafts",
                column: "SenderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_Status",
                table: "EmailDrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmailDrafts_TemplateId",
                table: "EmailDrafts",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailDraftAttachments");

            migrationBuilder.DropTable(
                name: "EmailDrafts");
        }
    }
}

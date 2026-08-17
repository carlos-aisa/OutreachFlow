using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignWorkspaceV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Body = table.Column<string>(type: "TEXT", maxLength: 20000, nullable: true),
                    SenderProfileId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FollowUpDueAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_SenderProfiles_SenderProfileId",
                        column: x => x.SenderProfileId,
                        principalTable: "SenderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CampaignAttachments",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttachmentAssetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignAttachments", x => new { x.CampaignId, x.AttachmentAssetId });
                    table.ForeignKey(
                        name: "FK_CampaignAttachments_AttachmentAssets_AttachmentAssetId",
                        column: x => x.AttachmentAssetId,
                        principalTable: "AttachmentAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAttachments_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignContactGroups",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactGroupId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignContactGroups", x => new { x.CampaignId, x.ContactGroupId });
                    table.ForeignKey(
                        name: "FK_CampaignContactGroups_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignContactGroups_ContactGroups_ContactGroupId",
                        column: x => x.ContactGroupId,
                        principalTable: "ContactGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignContacts",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignContacts", x => new { x.CampaignId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_CampaignContacts_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttachments_AttachmentAssetId",
                table: "CampaignAttachments",
                column: "AttachmentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContactGroups_ContactGroupId",
                table: "CampaignContactGroups",
                column: "ContactGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContacts_ContactId",
                table: "CampaignContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Name",
                table: "Campaigns",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_SenderProfileId",
                table: "Campaigns",
                column: "SenderProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignAttachments");

            migrationBuilder.DropTable(
                name: "CampaignContactGroups");

            migrationBuilder.DropTable(
                name: "CampaignContacts");

            migrationBuilder.DropTable(
                name: "Campaigns");
        }
    }
}

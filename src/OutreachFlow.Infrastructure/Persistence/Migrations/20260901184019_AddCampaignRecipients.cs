using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignRecipients : Migration
    {
        private static readonly string[] CampaignRecipientUniqueIndexColumns = { "CampaignId", "ContactId", "MessageTemplateId" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CampaignRecipientId",
                table: "FollowUpTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FollowUpDueDays",
                table: "Campaigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "FollowUpEnabled",
                table: "Campaigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FollowUpType",
                table: "Campaigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CampaignRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CampaignId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MessageTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailDraftId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExclusionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IncorporatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_EmailDrafts_EmailDraftId",
                        column: x => x.EmailDraftId,
                        principalTable: "EmailDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_EmailTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_CampaignRecipientId",
                table: "FollowUpTasks",
                column: "CampaignRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_CampaignId_ContactId_MessageTemplateId",
                table: "CampaignRecipients",
                columns: CampaignRecipientUniqueIndexColumns,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_ContactId",
                table: "CampaignRecipients",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_EmailDraftId",
                table: "CampaignRecipients",
                column: "EmailDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_MessageTemplateId",
                table: "CampaignRecipients",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_Status",
                table: "CampaignRecipients",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowUpTasks_CampaignRecipients_CampaignRecipientId",
                table: "FollowUpTasks",
                column: "CampaignRecipientId",
                principalTable: "CampaignRecipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowUpTasks_CampaignRecipients_CampaignRecipientId",
                table: "FollowUpTasks");

            migrationBuilder.DropTable(
                name: "CampaignRecipients");

            migrationBuilder.DropIndex(
                name: "IX_FollowUpTasks_CampaignRecipientId",
                table: "FollowUpTasks");

            migrationBuilder.DropColumn(
                name: "CampaignRecipientId",
                table: "FollowUpTasks");

            migrationBuilder.DropColumn(
                name: "FollowUpDueDays",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "FollowUpEnabled",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "FollowUpType",
                table: "Campaigns");
        }
    }
}

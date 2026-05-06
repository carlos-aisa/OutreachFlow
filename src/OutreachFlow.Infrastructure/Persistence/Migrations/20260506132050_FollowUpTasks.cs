using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FollowUpTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FollowUpTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DueAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowUpTasks_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowUpTasks_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_ContactId",
                table: "FollowUpTasks",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_ContactId_DueAt_IsCompleted",
                table: "FollowUpTasks",
                columns: new[] { "ContactId", "DueAt", "IsCompleted" });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_DueAt",
                table: "FollowUpTasks",
                column: "DueAt");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_IsCompleted",
                table: "FollowUpTasks",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpTasks_OrganizationId",
                table: "FollowUpTasks",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowUpTasks");
        }
    }
}

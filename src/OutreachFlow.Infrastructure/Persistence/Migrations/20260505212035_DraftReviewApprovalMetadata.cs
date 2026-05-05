using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutreachFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DraftReviewApprovalMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovedAt",
                table: "EmailDrafts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAt",
                table: "EmailDrafts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "EmailDrafts");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "EmailDrafts");
        }
    }
}

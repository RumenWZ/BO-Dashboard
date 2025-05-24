using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_AspNetUsers_ApplicationUserId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_ApplicationUserId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Service");

            migrationBuilder.CreateTable(
                name: "ApplicationUserService",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserService", x => new { x.UserId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserService_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserService_ServiceId",
                table: "ApplicationUserService",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserService");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Service",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ApplicationUserId",
                table: "Service",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_AspNetUsers_ApplicationUserId",
                table: "Service",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

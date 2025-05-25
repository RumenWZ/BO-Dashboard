using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserService_AspNetUsers_UserId",
                table: "ApplicationUserService");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserService_Services_ServiceId",
                table: "ApplicationUserService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserService",
                table: "ApplicationUserService");

            migrationBuilder.RenameTable(
                name: "ApplicationUserService",
                newName: "ApplicationUserServices");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserService_ServiceId",
                table: "ApplicationUserServices",
                newName: "IX_ApplicationUserServices_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserServices",
                table: "ApplicationUserServices",
                columns: new[] { "UserId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UserId",
                table: "ApplicationUserServices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_Services_ServiceId",
                table: "ApplicationUserServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UserId",
                table: "ApplicationUserServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_Services_ServiceId",
                table: "ApplicationUserServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserServices",
                table: "ApplicationUserServices");

            migrationBuilder.RenameTable(
                name: "ApplicationUserServices",
                newName: "ApplicationUserService");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserServices_ServiceId",
                table: "ApplicationUserService",
                newName: "IX_ApplicationUserService_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserService",
                table: "ApplicationUserService",
                columns: new[] { "UserId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserService_AspNetUsers_UserId",
                table: "ApplicationUserService",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserService_Services_ServiceId",
                table: "ApplicationUserService",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

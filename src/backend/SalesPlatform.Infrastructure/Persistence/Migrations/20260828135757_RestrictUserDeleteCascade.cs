using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestrictUserDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesRecords_Users_OwnerUserId",
                table: "SalesRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadBatches_Users_UploadedByUserId",
                table: "UploadBatches");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesRecords_Users_OwnerUserId",
                table: "SalesRecords",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadBatches_Users_UploadedByUserId",
                table: "UploadBatches",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesRecords_Users_OwnerUserId",
                table: "SalesRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadBatches_Users_UploadedByUserId",
                table: "UploadBatches");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesRecords_Users_OwnerUserId",
                table: "SalesRecords",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadBatches_Users_UploadedByUserId",
                table: "UploadBatches",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

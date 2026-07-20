using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImportTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportId",
                table: "library",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "import_history",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedBooks",
                table: "import_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImportedBooks",
                table: "import_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedBooks",
                table: "import_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "import_history",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "import_history",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalBooks",
                table: "import_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_library_ImportId",
                table: "library",
                column: "ImportId");

            migrationBuilder.AddForeignKey(
                name: "FK_library_import_history_ImportId",
                table: "library",
                column: "ImportId",
                principalTable: "import_history",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_library_import_history_ImportId",
                table: "library");

            migrationBuilder.DropIndex(
                name: "IX_library_ImportId",
                table: "library");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "library");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "FailedBooks",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "ImportedBooks",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "ProcessedBooks",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "import_history");

            migrationBuilder.DropColumn(
                name: "TotalBooks",
                table: "import_history");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculoInterativo.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCurriculumGenerationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurriculumGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobDescriptionPreview = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Success")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumGenerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurriculumGenerations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumGenerations_IpAddress_GeneratedAt",
                table: "CurriculumGenerations",
                columns: new[] { "IpAddress", "GeneratedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumGenerations_UserId_GeneratedAt",
                table: "CurriculumGenerations",
                columns: new[] { "UserId", "GeneratedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurriculumGenerations");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Database.Migrations
{
    /// <inheritdoc />
    public partial class SecurityEventAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityEvent",
                columns: table => new
                {
                    SecurityEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SessionGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemoteEndPoint = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Opcode = table.Column<int>(type: "int", nullable: true),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    CharacterName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.SecurityEvent", x => x.SecurityEventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvent_EventType",
                table: "SecurityEvent",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvent_TimestampUtc",
                table: "SecurityEvent",
                column: "TimestampUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityEvent");
        }
    }
}

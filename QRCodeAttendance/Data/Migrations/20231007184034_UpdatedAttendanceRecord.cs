using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeAttendance.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_AttendanceRecords_AttendanceRecordId",
                table: "StudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendances_AttendanceRecordId",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "AttendanceRecordId",
                table: "StudentAttendances");

            migrationBuilder.CreateTable(
                name: "AttendanceLibrary",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false),
                    AttendeesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceLibrary", x => new { x.AttendanceId, x.AttendeesId });
                    table.ForeignKey(
                        name: "FK_AttendanceLibrary_AttendanceRecords_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "AttendanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceLibrary_StudentAttendances_AttendeesId",
                        column: x => x.AttendeesId,
                        principalTable: "StudentAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLibrary_AttendeesId",
                table: "AttendanceLibrary",
                column: "AttendeesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceLibrary");

            migrationBuilder.AddColumn<int>(
                name: "AttendanceRecordId",
                table: "StudentAttendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_AttendanceRecordId",
                table: "StudentAttendances",
                column: "AttendanceRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_AttendanceRecords_AttendanceRecordId",
                table: "StudentAttendances",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id");
        }
    }
}

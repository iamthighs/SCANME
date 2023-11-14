using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeAttendance.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendanceRecordId",
                table: "StudentAttendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_AttendanceRecords_AttendanceRecordId",
                table: "StudentAttendances");

            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendances_AttendanceRecordId",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "AttendanceRecordId",
                table: "StudentAttendances");
        }
    }
}

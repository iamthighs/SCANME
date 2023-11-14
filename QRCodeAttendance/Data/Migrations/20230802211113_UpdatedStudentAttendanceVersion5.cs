using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeAttendance.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStudentAttendanceVersion5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "StudentAttendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeIn",
                table: "StudentAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOut",
                table: "StudentAttendances",
                type: "datetime2",
                nullable: true);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeAttendance.Data;
using QRCodeAttendance.Models;
using System.Diagnostics;

namespace QRCodeAttendance.Controllers
{
    [Authorize]
    [ApiController]
    [Route("AttendanceRecord/RecordForAttendance/api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(ILogger<AttendanceController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // New action to check if the QR code exists in the database
        [HttpGet("CheckQRCode")]
        public IActionResult CheckQRCode(string qrCodeData)
        {
            _logger.LogInformation("Received QR code data: {0}", qrCodeData);
            var student = _context.Samples.FirstOrDefault(s => s.Code == qrCodeData);
            if (student == null)
            {
                _logger.LogWarning("QR code data not found in the database.");
                return NotFound(); // QR code data not found in the database
            }

            return Ok(); // QR code data found in the database
        }

        // New action to check if the QR code exists in the database
        [HttpGet("GetStudentInfo")]
        public IActionResult GetStudentInfo(string qrCodeData)
        {
            var student = _context.Samples.FirstOrDefault(s => s.Code == qrCodeData);
            if (student == null)
            {
                return NotFound(); // QR code data not found in the database
            }

            return Json(new 
            {
                FirstName = student.FirstName.ToUpper(), 
                MiddleName = student.MiddleName.ToUpper(), 
                LastName = student.LastName.ToUpper(),
                ImageUrl = student.ImageUrl, 
                Sex = student.Sex.ToString().ToUpper(), 
                BirthDate = student.BirthDate.ToString("MMMM dd, yyyy").ToUpper(), 
                Department = student.Department.ToString(), 
                Course = student.Course.ToUpper()
            });
        }

        [HttpPost]
        public IActionResult RecordAttendance([FromBody] StudentAttendance record)
        {
            if (ModelState.IsValid)
            {
                // Create a new AttendanceRecord based on the data received from the client
                var attendanceRecord = new StudentAttendance
                {
                    Code = record.Code,
                    Status = record.Status,
                    Time = DateTime.Now
                };
                if (record.Status == "null" || record.Status == "In")
                {
                    TempData["success"] = "Time In";
                    // Add the attendance record to the database
                    _context.StudentAttendances.Add(attendanceRecord);
                    _context.SaveChanges();
                } else if (record.Status == "Out")
                {
                    TempData["success"] = "Time Out";
                    // Add the attendance record to the database
                    _context.StudentAttendances.Add(attendanceRecord);
                    _context.SaveChanges();
                }


                return Ok();
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { Errors = errors });
        }

        

    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCodeAttendance.Data;
using QRCodeAttendance.Models;

namespace QRCodeAttendance.Controllers
{
    [Authorize]
    public class AttendanceRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendanceRecordController> _logger;

        public AttendanceRecordController(ILogger<AttendanceRecordController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: AttendanceRecord
        public async Task<IActionResult> Index()
        {
            return _context.AttendanceRecords != null ?
                        View(await _context.AttendanceRecords.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.AttendanceRecord'  is null.");
        }

        // GET: AttendanceRecord/Details/5
        public IActionResult Details(int id)
        {
            var @record = _context.AttendanceRecords.Include(e => e.Attendees).FirstOrDefault(e => e.Id == id);
            return View(@record);
        }
      
        public IActionResult RecordForAttendance(int id)
        {
            var studentId = _context.StudentAttendances.Find(id);
            ViewBag.StudentId = studentId;

            var @record = _context.AttendanceRecords.Include(e => e.Attendees).FirstOrDefault(e => e.Id == id);
            return View(record);
        }
        
        [HttpPost("AddStudentToRecord")]
        public async Task<IActionResult> AddStudentToRecord(int recordId, [FromBody] StudentAttendance studentRecord)
        {
            var @record = _context.AttendanceRecords.Include(e => e.Attendees).FirstOrDefault(e => e.Id == recordId);
            var attendanceRecord = new StudentAttendance
            {
                Code = studentRecord.Code,
                Status = studentRecord.Status,
                Time = DateTime.Now
            };
            if (@record == null || attendanceRecord == null)
            {
                return NotFound();
            }

            if (!@record.Attendees.Contains(attendanceRecord))
            {
                if (attendanceRecord.Status == "null" || attendanceRecord.Status == "Out")
                {
                    TempData["success"] = "Time In";
                    // Add the attendance record to the database
                    _context.StudentAttendances.Add(attendanceRecord);
                    _context.SaveChanges();
                }
                @record.Attendees.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                TempData["success"] = "Successfully recorded in the Attendance List";
            }
            else if (@record.Attendees.Contains(attendanceRecord))
            {
                if (attendanceRecord.Status == "In")
                {
                    TempData["success"] = "Time Out";
                    attendanceRecord.Status = "Out";
                    // Add the attendance record to the database
                    _context.StudentAttendances.Add(attendanceRecord);
                    _context.SaveChanges();
                }
                @record.Attendees.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                TempData["success"] = "Successfully recorded in the Attendance List";
            }
            else
            {
                TempData["error"] = "Error when recording in the Attendance List";
                return RedirectToAction("RecordForAttendance", new { id = recordId });
            }

            return RedirectToAction("RecordForAttendance", new { id = recordId });
        }

        // POST: AttendanceRecord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,DateCreated")] AttendanceRecord attendanceRecord)
        {
            if (ModelState.IsValid)
            {
                attendanceRecord.DateCreated = DateTime.Now;
                _context.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                TempData["success"] = "Attendance Record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(attendanceRecord);
        }

        // POST: AttendanceRecord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DateCreated")] AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attendanceRecord.DateCreated = DateTime.ParseExact(attendanceRecord.DateCreated.ToString("yyyy-MM-ddTHH:mm"), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    _context.Update(attendanceRecord);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Attendance Record updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceRecordExists(attendanceRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(attendanceRecord);
        }

        // POST: AttendanceRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AttendanceRecords == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AttendanceRecord'  is null.");
            }
            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord != null)
            {
                _context.AttendanceRecords.Remove(attendanceRecord);
            }
            TempData["success"] = "Attendance Record deleted successfully.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteSelected(List<int> selectedIds)
        {
            if (_context.AttendanceRecords == null)
            {
                return Json(new { success = false, message = "Entity set 'ApplicationDbContext.AttendanceRecords' is null." });
            }

            var samplesToDelete = _context.AttendanceRecords.Where(sample => selectedIds.Contains(sample.Id)).ToList();

            _context.AttendanceRecords.RemoveRange(samplesToDelete);
            _context.SaveChanges();
            TempData["success"] = "Selected records deleted successfully.";
            return Json(new { success = true, message = "Selected records deleted successfully." });
        }
        private bool AttendanceRecordExists(int id)
        {
            return (_context.AttendanceRecords?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

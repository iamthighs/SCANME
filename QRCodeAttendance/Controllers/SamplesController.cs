using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCodeAttendance.Data;
using QRCodeAttendance.Models;
using QRCodeAttendance.Repositories;

namespace QRCodeAttendance.Controllers
{
    [Authorize]
    public class SamplesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BulkRepository _bulkRepository;
        private readonly SampleImportService _sampleImportService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SamplesController(ApplicationDbContext context, BulkRepository bulkRepository, SampleImportService sampleImportService, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _bulkRepository = bulkRepository;
            _sampleImportService = sampleImportService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Samples
        public async Task<IActionResult> Index()
        {
              return _context.Samples != null ? 
                          View(await _context.Samples.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Samples'  is null.");
        }

        private string UploadedFile(Sample studentModel)
        {
            string uniqueFileName = studentModel.ImageUrl;

            if (studentModel.CoverImage != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "profile");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + studentModel.CoverImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    studentModel.CoverImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
        // POST: Samples/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sample sample)
        {
            string uniqueFileName = UploadedFile(sample);
            sample.ImageUrl = uniqueFileName;

            string imgext = Path.GetExtension(sample.CoverImage.FileName);
            if (imgext == ".jpg" || imgext == ".png")
            {
                _context.Add(sample);
                await _context.SaveChangesAsync();
                TempData["success"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Uploaded file is not a jpg or png file!");
                TempData["error"] = "Uploaded file is not a jpg or png file!";
            }
            TempData["error"] = "There's an error!";
            
            return View();
        }

        
        // POST: Samples/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sample sample)
        {
            if (id != sample.Id)
            {
                return NotFound();
            }

            string uniqueFileName = UploadedFile(sample);
            sample.ImageUrl = uniqueFileName;
            string imgext = Path.GetExtension(uniqueFileName);
            if (imgext == ".jpg" || imgext == ".png")
            {
                _context.Update(sample);
                await _context.SaveChangesAsync();
                TempData["success"] = "Student updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Uploaded file is not a jpg or png file!");
                TempData["error"] = "Uploaded file is not a jpg or png file!";
            }
            
            return RedirectToAction(nameof(Index));
            
        }

        
        // POST: Samples/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Samples == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Samples'  is null.");
            }
            var sample = await _context.Samples.FindAsync(id);
            if (sample != null)
            {
                _context.Samples.Remove(sample);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "Student deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteSelected(List<int> selectedIds)
        {
            if (_context.Samples == null)
            {
                return Json(new { success = false, message = "Entity set 'ApplicationDbContext.Samples' is null." });
            }

            var samplesToDelete = _context.Samples.Where(sample => selectedIds.Contains(sample.Id)).ToList();

            _context.Samples.RemoveRange(samplesToDelete);
            _context.SaveChanges();
            TempData["success"] = "Selected records deleted successfully.";
            return Json(new { success = true, message = "Selected records deleted successfully." });
        }
        

        [HttpPost]
        public IActionResult BulkImportSamples(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                TempData["error"] = "Please select a valid file for import.";
                return RedirectToAction("Index"); // Redirect to the page with the import form.
            }

            try
            {
                // Parse the uploaded file and create a collection of Sample objects.
                var samples = _sampleImportService.ParseAndCreateSampleObjects(file);

                // Insert the samples into the database.
                _bulkRepository.BulkInsertSamples(samples);

                TempData["success"] = "Bulk import of students successful.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred during the bulk import: " + ex.Message;
            }

            return RedirectToAction("Index"); // Redirect back to the import form page.
        }

        



        private bool SampleExists(int id)
        {
          return (_context.Samples?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QRCodeAttendance.Controllers
{
    [Authorize]
    public class CameraSurveillanceController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CameraSurveillanceController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public IActionResult SaveCapturedImage([FromBody] string base64Image)
        {
            try
            {
                // Get the wwwroot path
                var webRootPath = _hostingEnvironment.WebRootPath;

                // Define a folder path within wwwroot to save the images
                var imagesFolder = Path.Combine(webRootPath, "captured-images");

                // Ensure the folder exists, create it if not
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // Generate a unique filename (you can customize the naming)
                var uniqueFileName = Guid.NewGuid().ToString() + ".png";

                // Combine the folder path and filename
                var imagePath = Path.Combine(imagesFolder, uniqueFileName);

                // Convert the base64 image to bytes
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                // Save the image to the specified path
                System.IO.File.WriteAllBytes(imagePath, imageBytes);

                // Optionally, you can return the saved image path or other data
                return Ok(new { imagePath = "/captured-images/" + uniqueFileName });
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                return BadRequest(new { error = ex.Message });
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

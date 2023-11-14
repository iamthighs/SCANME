using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace QRCodeAttendance.ViewComponents
{
    public class DetailsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default");
        }
    }
}

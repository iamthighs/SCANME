using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeAttendance.Data
{
    public class AppIdentityUser : IdentityUser
    {

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Profile Picture")]
        [NotMapped]
        public IFormFile? CoverImage { get; set; }
        public DateTime JoinedDate { get; set; }
    }



}

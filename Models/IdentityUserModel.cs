using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Custom IdentityUser model for adding first and last name to the Identity database
    /// </summary>
    public class IdentityUserModel : IdentityUser
    {

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is a required field")]
        [StringLength(64, ErrorMessage = "First Name must be 1-64 characters", MinimumLength = 1)]
        [PersonalData]
        public string first_name { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is a required field")]
        [StringLength(64, ErrorMessage = "Last Name must be 1-64 characters", MinimumLength = 1)]
		[PersonalData]
		public string last_name { get; set; }


    }
}

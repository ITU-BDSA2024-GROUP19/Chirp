using Chirp.Core;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    public class ProfilePictureModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ICheepService _cheepService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _environment;

        public ProfilePictureModel(
            UserManager<Author> userManager,
            ILogger<PersonalDataModel> logger,
            ICheepService cheepService,
            IAuthorService authorService,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _logger = logger;
            _cheepService = cheepService;
            _authorService = authorService;
            _environment = environment;
        }

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }
        public string ProfilePictureUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            ProfilePictureUrl = _authorService.GetProfilePicture(user.UserName!);
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (ProfilePicture == null)
            {
                ModelState.AddModelError("Input.ProfilePicture",
                    "Please select a file.");
                return RedirectToPage();
            }
            
            if (ProfilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(ProfilePicture.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Input.ProfilePicture",
                        "Only .jpg, .jpeg, and .png files are allowed.");
                    return Page();
                }

                if (ProfilePicture.Length > 2 * 1024 * 1024) // Limit to 2 MB
                {
                    ModelState.AddModelError("Input.ProfilePicture",
                        "The file size must be less than 2 MB.");
                    return Page();
                }
                
                _authorService.UpdateProfilePicture(user.UserName!, ProfilePicture.OpenReadStream());
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            _authorService.DeleteProfilePicture(user.UserName!);
            
            return RedirectToPage();
        }
    }
    
    
}
﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

using Chirp.Core;
using Chirp.Infrastructure.Authors;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthorService _chirpAccountService;
        private readonly SignInManager<Author> _signInManager;
        private readonly UserManager<Author> _userManager;
        private readonly IUserStore<Author> _userStore;
        private readonly IUserEmailStore<Author> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _environment;

        public RegisterModel(
            IAuthorService chirpAccountService,
            UserManager<Author> userManager,
            IUserStore<Author> userStore,
            SignInManager<Author> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment environment)
        {
            _chirpAccountService = chirpAccountService;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _environment = environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(60, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public IFormFile ProfilePicture { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var addAuthorResult = await _chirpAccountService.AddAuthor(Input.UserName, Input.Email, Input.Password);
                var result = addAuthorResult.IdentityResult;
                var user = addAuthorResult.User;

                if (result.Succeeded)
                {
                    if (Input.ProfilePicture != null)
                    {
                        // Validate and save the profile picture
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(Input.ProfilePicture.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Input.ProfilePicture",
                                "Only .jpg, .jpeg, and .png files are allowed.");
                            ReturnUrl = returnUrl;
                            return Page();
                        }

                        if (Input.ProfilePicture.Length > 2 * 1024 * 1024) // Limit to 2 MB
                        {
                            ModelState.AddModelError("Input.ProfilePicture",
                                "The file size must be less than 2 MB.");
                            ReturnUrl = returnUrl;
                            return Page();
                        }
                        _chirpAccountService.UpdateProfilePicture(Input.UserName, Input.ProfilePicture.OpenReadStream());
                    }
                    _logger.LogInformation("User created a new account with password."); // Logs some info.
                    await SendConfirmationEmail(user);

                    // If account confirmation is required, we need to show the link if we don't have a real email sender
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SendConfirmationEmail(Author user)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private Author CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Author>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Author)}'. " +
                    $"Ensure that '{nameof(Author)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Author> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Author>)_userStore;
        }
    }
}

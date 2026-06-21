// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.Identity_Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace PocketCartApp.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<PocketCartApplicationUser> _signInManager;
        private readonly UserManager<PocketCartApplicationUser> _userManager;
        private readonly IUserStore<PocketCartApplicationUser> _userStore;
        private readonly IUserEmailStore<PocketCartApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<PocketCartApplicationUser> userManager,
            IUserStore<PocketCartApplicationUser> userStore,
            SignInManager<PocketCartApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<SelectListItem> RolesList { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            public DateTime StartDate { get; set; }

            [Required]
            public Contract_Type Contract_Type { get; set; }

            public int? duration { get; set; }

            public DateTime? EndDate { get; set; }

            public string EmployeeId { get; set; }

            public Account_Status Account_Status { get; set; }

            [Required]
            public string Role { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        private void LoadRoles()
        {
            RolesList = new List<SelectListItem>
            {
                new SelectListItem { Value = Roles.Admin, Text = Roles.Admin },
                new SelectListItem { Value = Roles.Manager, Text = Roles.Manager },
                new SelectListItem { Value = Roles.Cashier, Text = Roles.Cashier }
            };
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            LoadRoles();

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Console.WriteLine("REGISTER POST HIT");

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            LoadRoles();

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input.StartDate < DateTime.Today)
            {
                ModelState.AddModelError("Input.StartDate", "Start date cannot be in the past.");
            }

            if (Input.Contract_Type == Contract_Type.Fixed)
            {
                if (!Input.duration.HasValue || Input.duration <= 0)
                {
                    ModelState.AddModelError("Input.duration", "Duration is required for fixed contracts.");
                }
                else
                {
                    Input.EndDate = Input.StartDate.AddMonths(Input.duration.Value);
                }
            }
            else
            {
                Input.EndDate = null;
            }

            if (string.IsNullOrWhiteSpace(Input.Role))
            {
                ModelState.AddModelError("Input.Role", "Please select a role.");
            }
            else if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                ModelState.AddModelError("Input.Role", "Selected role does not exist.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = CreateUser();

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;

            string first = Input.FirstName.Trim().ToLower();
            string last = Input.LastName.Trim().ToLower();

            string customUserName = $"{first}.{last}@pocketcart.com";
            int counter = 1;

            while (await _userManager.FindByNameAsync(customUserName) != null)
            {
                customUserName = $"{first}.{last}{counter}@pocketcart.com";
                counter++;
            }

            var year = DateTime.Now.Year;

            var lastEmployee = _userManager.Users
                .Where(u => u.EmployeeId != null && u.EmployeeId.StartsWith($"EMP-{year}-"))
                .OrderByDescending(u => u.EmployeeId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastEmployee != null)
            {
                var lastNumber = int.Parse(lastEmployee.EmployeeId.Split('-').Last());
                nextNumber = lastNumber + 1;
            }

            user.EmployeeId = $"EMP-{year}-{nextNumber:D4}";
            user.StartDate = Input.StartDate;
            user.contract_Type = Input.Contract_Type;
            user.EndDate = Input.EndDate;
            user.Account_Status = Input.Account_Status;

            await _userStore.SetUserNameAsync(user, customUserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine("CREATE ERROR: " + error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            user.ShoppingCart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                CashierOnShift = user.Id
            };

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User created a new account with password and role {Role}.", Input.Role);

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl },
                protocol: Request.Scheme);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email sending failed for {Email}", Input.Email);
                }
            });

            TempData["Success"] = "User created successfully.";

            return RedirectToAction("Index", "Employees");
        }

        private PocketCartApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<PocketCartApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(PocketCartApplicationUser)}'.");
            }
        }

        private IUserEmailStore<PocketCartApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<PocketCartApplicationUser>)_userStore;
        }
    }
}
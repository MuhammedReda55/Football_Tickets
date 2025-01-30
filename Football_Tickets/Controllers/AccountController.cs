using Football_Tickets.Models;
using Football_Tickets.Models.ViewModels;
using Football_Tickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace Football_Tickets.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender emailSender;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser>
            signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.emailSender = emailSender;

        }
        public async Task<IActionResult> Register()
        {
            ViewBag.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View();
        }
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Register));
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Register));
            }

            // Sign in the user with this external login provider if the user already has a login
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (signInResult.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl });
            }

            if (signInResult.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }

            // If the user does not have an account, then create one and sign in
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var user = new ApplicationUser { UserName = email, Email = email };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            ViewData["ErrorMessage"] = "Error signing in with external provider.";
            return RedirectToAction(nameof(Register));
        }


        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUserVM UserVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = UserVM.UserName,
                    Email = UserVM.Email,
                    Address = UserVM.Address,
                    Name = UserVM.Name,
                    photo = "~/default-photo.png"
                };

                //ApplicationUser user = _mapper.Map<ApplicationUser>(UserVM);

                var result = await userManager.CreateAsync(user, UserVM.Password);
                if (result.Succeeded)
                {
                    // Add user to the "Customer" role
                    await userManager.AddToRoleAsync(user, SD.UserRole);

                    //                    // Generate the login URL
                    //                    string loginUrl = Url.Action("Login", "Account", null, protocol: Request.Scheme);

                    //                    // Prepare the email content
                    //                    string emailContent = $@"
                    //<html>
                    //<body>
                    //    <h1>Registration Successful!</h1>
                    //    <p>Thank you for registering with us!</p>
                    //    <p>Click the link below to log in:</p>
                    //    <a href='{loginUrl}'>Go to Login</a>
                    //</body>
                    //</html>";

                    //                    // Send the email
                    //                    await emailSender.SendEmailAsync(user.Email, "Registration Successful", emailContent);

                    //                    // Redirect to the Register page
                    //                    return RedirectToAction("Register", "Account");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("Password", "يرجى ادخال كلمه سر تحتوي على احرف كبيره وصغيره وارقام وحروف مميزه");
                }
            }

            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(loginVM.Account) ?? await userManager.FindByNameAsync(loginVM.Account);


                if (user != null)
                {
                    if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is locked.");
                        return View(loginVM);
                    }
                    var result = await userManager.CheckPasswordAsync(user, loginVM.Password);

                    if (result)
                    {
                        await signInManager.SignInAsync(user, loginVM.RemeberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "هناك خطأ في الحساب او في كلمه السر");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "البريد الالكتروني غير موجود");
                }
            }

            return View(loginVM);
        }
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult CheckEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckEmail(ForgotPasswordVM forgotPasswordVM)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني غير موجود.");
                return View(forgotPasswordVM);
            }
            TempData["Email"] = forgotPasswordVM.Email;
            return RedirectToAction("ForgetPassword", "Account");
        }
        public IActionResult ForgetPassword()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {

                var email = TempData["Email"] as string;
                if (email == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    return RedirectToAction("CheckEmail");
                }

                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    return View();
                }

                var result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, forgotPasswordVM.NewPassword);
                    TempData["message"] = "تم تغيير كلمة المرور بنجاح.";
                    return RedirectToAction("Login", "Account");

                }

                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إزالة كلمة المرور القديمة.");
                return View(forgotPasswordVM);

            }
            return View(forgotPasswordVM);

        }
        public IActionResult ChangePassword()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChanagePasswordVM chanagePasswordVM)
        {

            if (ModelState.IsValid)
            {

                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    //return RedirectToAction("CheckEmail");
                }

                //var user = await userManager.FindByEmailAsync(email);

                //if (user == null)
                //{
                //    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                //    return View();
                //}
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, chanagePasswordVM.OldPassword);
                if (!checkPasswordResult)
                {
                    ModelState.AddModelError("OldPassword", "كلمة السر القديمة غير صحيحة");
                    return View(chanagePasswordVM);
                }
                var result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, chanagePasswordVM.NewPassword);
                    TempData["message"] = "تم تغيير كلمة المرور بنجاح.";
                    return RedirectToAction("Profile", "Account");

                }

                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إزالة كلمة المرور القديمة.");
                return View(chanagePasswordVM);

            }
            return View(chanagePasswordVM);

        }
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            return View(model: new ApplicationUserVM()
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ApplicationUserVM userVM)
        {
            var user = await userManager.GetUserAsync(User);
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            user.Name = userVM.Name;
            user.Address = userVM.Address;

            await userManager.UpdateAsync(user);
            TempData["message"] = "تم تحديث البيانات بنجاح";

            return RedirectToAction("Profile", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> updatePhoto(IFormFile photo)
        {
            var user = await userManager.GetUserAsync(User);

            if (photo != null && photo.Length > 0)
            {
                // Genereate name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);

                // Save in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Photo_Users", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    photo.CopyTo(stream);
                }

                // Save in db
                user.photo = fileName;
                await userManager.UpdateAsync(user);
            }

            TempData["message"] = "تم تحديث البيانات بنجاح";

            return RedirectToAction("Profile", "Account");
        }
        public async Task<IActionResult> Users()
        {
            var users = userManager.Users.ToList();


            var userRoles = new List<UserWithRolesVM>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRolesVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    Roles = roles.ToList(),
                    IsBlocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow
                });
            }

            return View(userRoles);
        }
        [HttpPost]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                {

                    user.LockoutEnd = null;
                }
                else
                {
                    user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                }

                await userManager.UpdateAsync(user);

                TempData["message"] = $"User {user.UserName} status updated.";

            }


            return RedirectToAction("Users");
        }
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await userManager.FindByIdAsync(id);


            if (user != null)
            {

                var result = await userManager.DeleteAsync(user);


                if (result.Succeeded)
                {
                    TempData["message"] = $"User {user.UserName} is Deleted Sueccessfully";
                    return RedirectToAction("Users");
                }
                else
                {
                    return BadRequest("Error deleting user.");
                }
            }


            return NotFound("User not found.");
        }
        [HttpPost]
        
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            //// التحقق من صحة الـ Role الجديد
            //var validRoles = new List<string> { SD.AdminRole, SD.UserRole, SD.EmployeeRole };
            //if (!validRoles.Contains(newRole))
            //{
            //    return BadRequest("Invalid role.");
            //}

            // البحث عن المستخدم
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // إزالة الأدوار القديمة وإضافة الدور الجديد
            var currentRoles = await userManager.GetRolesAsync(user);
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return BadRequest("Error while removing old roles.");
            }

            var addResult = await userManager.AddToRoleAsync(user, role);

            if (addResult.Succeeded)
            {
                TempData["message"] = $"User {user.UserName} role updated to {role} successfully.";
                return RedirectToAction("Users");
            }
            else
            {
                return BadRequest("Error while updating role.");
            }
        }



    }
}

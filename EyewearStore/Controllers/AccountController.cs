using EyewearStore.Enums;
using EyewearStore.Models;
using EyewearStore.Services;
using EyewearStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EyewearStore.Controllers;


public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailService = emailService;
    }

    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (!ModelState.IsValid)
            return View();
        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Wrong Email or Password");
            return View(vm);
        }
        if (!user.EmailConfirmed)
        {

            ModelState.AddModelError("", "Email not confirm,please check your inbox");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(user, vm.Password, true, true);

        if (!result.Succeeded)
        {

            ModelState.AddModelError("", "Wrong Email or Password");
            return View(vm);
        }


        return RedirectToAction("Index", "Home");
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
            return View();

        AppUser newUser = new AppUser
        {
            Fullname = vm.Fullname,
            Email = vm.Email,
            UserName = vm.Username
        };
        var result = await _userManager.CreateAsync(newUser, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(vm);
        }
        var resultRole = await _userManager.AddToRoleAsync(newUser, Roles.Admin.ToString());

        if (!resultRole.Succeeded)
        {

            foreach (var error in resultRole.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(vm);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        var link = Url.Action("VerifyEmail", "Account", new { email = newUser.Email, token = token }, HttpContext.Request.Scheme);


        string emailBody = @$"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirm Your Email Address</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 50px auto;
            background-color: rgb(226, 198, 198);
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .message {{
            color: #333;
            margin-bottom: 20px;
        }}
        .confirmation-link {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            border-radius: 5px;
        }}
        .confirmation-link:hover {{
            background-color: #0056b3;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <p class=""message"">Dear User,</p>
        <p class=""message"">Please click the following link to confirm your email address and complete your registration:</p>
        <a href=""{link}"" class=""confirmation-link"">Confirm Email</a>
        <p class=""message"">If you did not request this, please ignore this email.</p>
        <p class=""message"">Regards,<br>Your Website Team</p>
    </div>
</body>
</html>";

        _emailService.SendEmail(new EmailDto(body: emailBody, subject: "Email Verification", to: vm.Email));
        TempData["VerifyEmail"] = "Confirmation mail sent!";

        return RedirectToAction("Login");
    }


    public async Task<IActionResult> VerifyEmail(string? email, string? token)
    {
        if (token == null || email == null) return NotFound();
        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Email == email);

        var verify = await _userManager.ConfirmEmailAsync(user, token);
        if (verify.Succeeded)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Register", "Home");
    }


    public async Task<IActionResult> CreateRole()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = role.ToString(),
            });
        }
        return RedirectToAction("Index", "Home");
    }



    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}

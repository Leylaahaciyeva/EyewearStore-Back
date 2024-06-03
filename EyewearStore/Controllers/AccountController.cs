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
        _emailService.SendEmail(new EmailDto(body: link, subject: "Email Verification", to: vm.Email));
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

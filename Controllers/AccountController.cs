using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Attributes;
using Inventory.Services;

namespace Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserService userService, SignInManager<ApplicationUser> signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
        }

        // Kayıt formu
        [OnlyAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OnlyAnonymous]
        public async Task<IActionResult> Register(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                // Kullanıcıyı oluştur
                var result = await _userService.Add(user, model.Password);
                if (result.Succeeded)
                {
                    return Redirect(nameof(Login));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Giriş formu
        [OnlyAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Giriş işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OnlyAnonymous]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {

                var user = await _userService.GetByUserName(model.UserName);
                if (user != null)
                {
                    if (await _userService.IsActive(user))
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                        if (result.Succeeded)
                        {
                            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");
                            return Redirect(returnUrl);
                        }
                        else if (result.IsLockedOut)
                        {
                            ModelState.AddModelError(string.Empty, "Hesap kilitli. Lütfen daha sonra tekrar deneyiniz.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Bu hesap pasif durumdadır!");
                    }
                }

                if (ModelState.ErrorCount == 0)
                {
                    ModelState.AddModelError(string.Empty, "Giriş bilgileri geçersiz!");
                }
            }

            return View(model);
        }

        // Çıkış işlemi
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}

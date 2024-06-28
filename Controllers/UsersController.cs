using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Listeleme işlemi
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();

        return View(users);
    }

    // Silme işlemi
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        // Kayıt mevcut değilse hata göster
        if (user == null)
        {
            return NotFound();
        }

        // Admin kendisini silemez
        if (user.UserName.Equals(User?.Identity?.Name))
        {
            return BadRequest();
        }

        try
        {
            await _userManager.DeleteAsync(user);
        }
        catch { }

        return RedirectToAction(nameof(Index));
    }

    // Düzenleme formu
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserEditViewModel
        {
            UserDetails = user,
            UserRoles = await _userManager.GetRolesAsync(user), // Kullanıcının sahip olduğu roller
            AllRoles = await GetAllRoles()  // Sistemdeki uygun roller
        };

        return View(model);
    }

    // Düzenleme işlemi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserDetails.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.UserDetails.FirstName;
            user.LastName = model.UserDetails.LastName;
            user.Email = model.UserDetails.Email;

            model.AllRoles = await GetAllRoles();

            try
            {
                IdentityResult result;

                { // Kullanıcı bilgileri
                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Kullanıcı bilgileri güncellenemedi!");
                    }
                }

                if (!string.IsNullOrEmpty(model.UserPassword)) // Parola
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    result = await _userManager.ResetPasswordAsync(user, token, model.UserPassword);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Parola güncellenemedi!");
                    }
                }

                { // Roller

                    var userRoles = await _userManager.GetRolesAsync(user);
                    var selectedRoles = model.UserRoles ?? [];

                    // Eğer admin yetkisi verilmiş ise tüm yetkiler verilmeli
                    if (selectedRoles.Contains(nameof(Roles.Admin)))
                    {
                        selectedRoles = model.AllRoles;
                    }

                    // Admin kendi yetkilerini kaldıramaz
                    if (user.UserName.Equals(User?.Identity?.Name))
                    {
                        selectedRoles = model.AllRoles;
                    }

                    result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray());
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Yeni roller eklenmemedi!");
                    }

                    result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray());
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Eski roller kaldırılamadı!");
                    }
                }
            }
            catch
            {
                return View("Error");
            }
            
                // Hiç hata yoksa flash mesajı göster
            if (ModelState.ErrorCount == 0)
            {
                TempData["Status"] = "success";
                TempData["Message"] = "Kullanıcı güncellendi.";
            }
        }

        return View(model);
    }

    private async Task<IList<string>> GetAllRoles()
    {
        return await _roleManager.Roles.Select(r => r.Name ?? "").ToListAsync() ?? [];
    }

}

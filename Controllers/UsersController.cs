using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models;
using Inventory.ViewModels;

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

    public IActionResult Index()
    {
        var users = _userManager.Users.Select(u => new UserViewModel
        {
            Id = u.Id,
            UserName = u.UserName,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        }).ToList();

        return View(users);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserEditViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        };

        ViewData["Roles"] = _roleManager.Roles.Select(r => r.Name).ToList();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles ?? new string[] { };

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add roles");
                return View(model);
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove roles");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["Roles"] = _roleManager.Roles.Select(r => r.Name).ToList();
        return View(model);
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


        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }

}

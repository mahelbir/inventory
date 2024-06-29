using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Services;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserService _userService;
    private readonly RoleService _roleService;

    public UsersController(UserService userService, RoleService roleService)
    {
        _roleService = roleService;
        _userService = userService;
    }

    // Listeleme işlemi
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAll();
        var userListViewModel = new List<UserListItemViewModel>();

        foreach (var user in users)
        {
            userListViewModel.Add(new UserListItemViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = await _userService.IsActive(user),
            });
        }

        return View(userListViewModel);
    }

    // Silme işlemi
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetById(id);

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
            await _userService.Delete(user);
        }
        catch { }

        return RedirectToAction(nameof(Index));
    }

    // Düzenleme formu
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserEditViewModel
        {
            UserDetails = user,
            UserRoles = await _userService.GetRoles(user), // Kullanıcının sahip olduğu roller
            AllRoles = await _roleService.GetAll()  // Sistemdeki uygun roller
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
            var user = await _userService.GetById(model.UserDetails.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.UserDetails.FirstName;
            user.LastName = model.UserDetails.LastName;
            user.Email = model.UserDetails.Email;

            model.AllRoles = await _roleService.GetAll();

            try
            {
                IdentityResult result;

                { // Kullanıcı bilgileri
                    result = await _userService.Update(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Kullanıcı bilgileri güncellenemedi!");
                    }
                }

                if (!string.IsNullOrEmpty(model.UserPassword)) // Parola
                {
                    result = await _userService.UpdatePassword(user, model.UserPassword);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Parola güncellenemedi!");
                    }
                }

                { // Roller

                    var userRoles = await _userService.GetRoles(user);
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

                    result = await _userService.AddRoles(user, selectedRoles.Except(userRoles).ToArray());
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Yeni roller eklenmemedi!");
                    }

                    result = await _userService.RemoveRoles(user, userRoles.Except(selectedRoles).ToArray());
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

    // Durum değiştirme işlemi
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userService.GetById(id);

        // Kayıt mevcut değilse hata göster
        if (user == null)
        {
            return NotFound();
        }

        // Admin kendisine müdahele edemez
        if (user.UserName.Equals(User?.Identity?.Name))
        {
            return BadRequest();
        }

        try
        {
            if (await _userService.IsActive(user))
            {
                await _userService.Deactivate(user);
            }
            else
            {
                await _userService.Activate(user);
            }
        }
        catch { }

        return RedirectToAction(nameof(Index));
    }

}

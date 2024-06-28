using Inventory.Models;
using Inventory.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Anasayfa
        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel();
            if (User?.Identity?.IsAuthenticated == true)
            {
                viewModel.User = await _userManager.GetUserAsync(User);
            }

            return View(viewModel);
        }

        // HTTP hata sayfalar�
        public IActionResult Error(int id = 0)
        {
            string message;
            switch (id)
            {
                case 400:
                    message = "Ge�ersiz istek!";
                    break;
                case 401:
                    message = "Yetkisiz eri�im!";
                    break;
                case 403:
                    message = "Eri�im engellendi!";
                    break;
                case 404:
                    message = "Sayfa bulunamad�!";
                    break;
                case 500:
                    message = "Sunucu hatas�!";
                    break;
                case 502:
                    message = "Sunucudan ge�ersiz yan�t!";
                    break;
                case 503:
                    message = "Sunucu kullan�lam�yor!";
                    break;
                case 504:
                    message = "A� ge�idi zaman a��m�na u�rad�!";
                    break;
                default:
                    message = "Bir hata olu�tu!";
                    break;
            }
            return View("Error", new ErrorViewModel { Message = message }); ;
        }

    }
}

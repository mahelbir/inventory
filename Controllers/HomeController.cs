using Inventory.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    public class HomeController : Controller
    {

        // Anasayfa
        public IActionResult Index()
        {
            return View();
        }

        // HTTP hata sayfalarý
        public IActionResult Error(int code = 0)
        {
            string message;
            switch (code)
            {
                case 400:
                    message = "Geçersiz istek!";
                    break;
                case 401:
                    message = "Yetkisiz eriþim!";
                    break;
                case 403:
                    message = "Eriþim engellendi!";
                    break;
                case 404:
                    message = "Sayfa bulunamadý!";
                    break;
                case 500:
                    message = "Sunucu hatasý!";
                    break;
                case 502:
                    message = "Sunucudan geçersiz yanýt!";
                    break;
                case 503:
                    message = "Sunucu kullanýlamýyor!";
                    break;
                case 504:
                    message = "Að geçidi zaman aþýmýna uðradý!";
                    break;
                default:
                    message = "Bir hata oluþtu!";
                    break;
            }
            return View("Error", new ErrorViewModel { Message = message }); ;
        }
    }
}

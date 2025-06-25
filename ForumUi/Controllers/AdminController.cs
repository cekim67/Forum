using Microsoft.AspNetCore.Mvc;

namespace ForumUi.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Session kontrolü
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            // Login kontrolü
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Bu sayfayı görüntülemek için giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Auth");
            }

            // Admin yetkisi kontrolü - case insensitive
            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["LoginMessage"] = $"Bu sayfayı görüntülemek için admin yetkisine sahip olmanız gerekiyor. Mevcut yetki: {role ?? "Belirsiz"}";
                return RedirectToAction("Index", "Topics");
            }

            return View();
        }

        public IActionResult ManageUsers()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Bu sayfayı görüntülemek için giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["LoginMessage"] = "Bu sayfayı görüntülemek için admin yetkisine sahip olmanız gerekiyor.";
                return RedirectToAction("Index", "Topics");
            }

            // Burada kullanıcı listesini API'dan çekebilirsiniz
            return View();
        }

        public IActionResult ModerateContent()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Bu sayfayı görüntülemek için giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["LoginMessage"] = "Bu sayfayı görüntülemek için admin yetkisine sahip olmanız gerekiyor.";
                return RedirectToAction("Index", "Topics");
            }

            // Burada içerik listesini API'dan çekebilirsiniz
            return View();
        }
    }
}
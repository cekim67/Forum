using ForumUi.Models;
using ForumUi.Views.Account;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace ForumWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Eğer zaten giriş yapılmışsa ana sayfaya yönlendir
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Topics");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7172/api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı.");
                return View(model);
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonConvert.DeserializeObject<TokenResponse>(responseData);

            // Token'ı session'a kaydet
            HttpContext.Session.SetString("JWToken", tokenObj!.Token);

            // ÖNEMLİ: Kullanıcı bilgilerini al ve UserRole'ü kaydet
            await GetAndSetUserRole(client, tokenObj.Token);

            TempData["Success"] = "Başarıyla giriş yaptınız!";
            return RedirectToAction("Index", "Topics");
        }

        private async Task GetAndSetUserRole(HttpClient client, string token)
        {
            try
            {
                // Authorization header ekle
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Kullanıcı bilgilerini API'dan al
                var userResponse = await client.GetAsync("https://localhost:7172/api/User/profile");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userData = await userResponse.Content.ReadAsStringAsync();
                    var userObj = JsonConvert.DeserializeObject<UserInfo>(userData);

                    // UserRole'ü session'a kaydet
                    if (!string.IsNullOrEmpty(userObj?.Role))
                    {
                        HttpContext.Session.SetString("UserRole", userObj.Role);
                    }

                    // Kullanıcı adını da kaydedebilirsiniz
                    if (!string.IsNullOrEmpty(userObj?.UserName))
                    {
                        HttpContext.Session.SetString("UserName", userObj.UserName);
                    }
                }
                else
                {
                    // Eğer API'dan kullanıcı bilgisi alınamazsa, varsayılan role ata
                    HttpContext.Session.SetString("UserRole", "User");
                }
            }
            catch (Exception)
            {
                // Hata durumunda varsayılan role ata
                HttpContext.Session.SetString("UserRole", "User");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Clear();
            TempData["Success"] = "Başarıyla çıkış yaptınız!";
            return RedirectToAction("Index", "Topics");
        }

        public class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
            public string? Role { get; set; } // API'dan role bilgisi geliyorsa
            public string? UserName { get; set; } // API'dan username geliyorsa
        }

        public class UserInfo
        {
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                var token = json["token"]?.ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("JWToken", token);

                    // Kayıt sonrası da kullanıcı bilgilerini al
                    await GetAndSetUserRole(client, token);
                }

                return RedirectToAction("Index", "Topics");
            }

            ViewBag.Error = "Kayıt başarısız.";
            return View(model);
        }
    }
}
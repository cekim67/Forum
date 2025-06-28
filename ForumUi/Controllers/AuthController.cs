using ForumUi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ForumUi.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            // Eğer zaten giriş yapmışsa ana sayfaya yönlendir
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Index", "Topics");
            }

            return View(new LoginViewModel());
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            var dto = new LoginDto
            {
                Email = viewModel.Email,
                Password = viewModel.Password
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("/api/Auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Giriş başarısız. Bilgileri kontrol et. ({response.StatusCode})";
                    return View(viewModel);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenObj = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                if (tokenObj?.Token == null)
                {
                    ViewBag.Error = "Token alınamadı.";
                    return View(viewModel);
                }

                // Token'ı session'a kaydet
                HttpContext.Session.SetString("JWToken", tokenObj.Token);

                // Kullanıcı bilgilerini /api/Users/me endpoint'inden al
                try
                {
                    var userClient = _httpClientFactory.CreateClient();
                    userClient.BaseAddress = new Uri("https://localhost:7172");
                    userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenObj.Token);

                    var userResponse = await userClient.GetAsync("/api/Users/me");

                    if (userResponse.IsSuccessStatusCode)
                    {
                        var userJson = await userResponse.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<UserAdminDto>(userJson);

                        if (userInfo != null)
                        {
                            // Admin durumunu session'a kaydet
                            HttpContext.Session.SetString("IsAdmin", userInfo.IsAdmin ? "1" : "0");

                            // Kullanıcı adını da session'a kaydet
                            HttpContext.Session.SetString("Username", userInfo.Username);
                            HttpContext.Session.SetString("UserId", userInfo.Id.ToString());
                        }
                    }
                    else
                    {
                        // Kullanıcı bilgileri alınamazsa, default olarak admin değil kabul et
                        HttpContext.Session.SetString("IsAdmin", "0");
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda log'la ve default değerler kullan
                    Console.WriteLine($"Kullanıcı bilgileri alınamadı: {ex.Message}");
                    HttpContext.Session.SetString("IsAdmin", "0");
                }

                return RedirectToAction("Index", "Topics");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = $"API bağlantı hatası: {ex.Message}";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Beklenmeyen hata: {ex.Message}";
                return View(viewModel);
            }
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            // ViewModel'i DTO'ya çevir
            var dto = new RegisterDto
            {
                Username = viewModel.Username,
                Email = viewModel.Email,
                Password = viewModel.Password
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("/api/Auth/register", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Kayıt başarısız. Lütfen tekrar deneyin. ({response.StatusCode})";
                    return View(viewModel);
                }

                TempData["Success"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = $"API bağlantı hatası: {ex.Message}";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Beklenmeyen hata: {ex.Message}";
                return View(viewModel);
            }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
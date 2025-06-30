using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ForumUi.Models; 
using System.Net.Http.Headers;
using System.Net.Http;

namespace ForumUi.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AdminController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private bool IsUserAdmin()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            return !string.IsNullOrEmpty(token) && isAdmin == "1";
        }

        private HttpClient CreateAuthorizedClient()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _clientFactory.CreateClient(); 
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> Dashboard()
        {
            
            if (!IsUserAdmin())
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                using var client = CreateAuthorizedClient();
                var statsResponse = await client.GetAsync("/api/Admin/stats");

                if (!statsResponse.IsSuccessStatusCode)
                {
                    var errorContent = await statsResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"İstatistikler alınamadı! Status: {statsResponse.StatusCode}, Error: {errorContent}";
                    return View(new ForumStatsDto());
                }

                var json = await statsResponse.Content.ReadAsStringAsync();
                var stats = JsonConvert.DeserializeObject<ForumStatsDto>(json);

                return View(stats ?? new ForumStatsDto());
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"API bağlantı hatası: {httpEx.Message}";
                return View(new ForumStatsDto());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(new ForumStatsDto());
            }
        }

        public async Task<IActionResult> Users()
        {
            
            if (!IsUserAdmin())
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                using var client = CreateAuthorizedClient();
                var response = await client.GetAsync("/api/Admin/users");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Kullanıcılar alınamadı! Status: {response.StatusCode}, Error: {errorContent}";
                    return View(new List<UserAdminDto>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserAdminDto>>(json);

                return View(users ?? new List<UserAdminDto>());
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"API bağlantı hatası: {httpEx.Message}";
                return View(new List<UserAdminDto>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(new List<UserAdminDto>());
            }
        }

       
        public async Task<IActionResult> Topics()
        {
            if (!IsUserAdmin())
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                using var client = CreateAuthorizedClient();
                var response = await client.GetAsync("/api/Admin/topics/all");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Konular alınamadı! Status: {response.StatusCode}";
                    return View(new List<TopicDto>()); 
                }

                var json = await response.Content.ReadAsStringAsync();
                var topics = JsonConvert.DeserializeObject<List<TopicDto>>(json); 

                return View(topics ?? new List<TopicDto>()); 
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(new List<TopicDto>()); 
            }
        }

        
        public async Task<IActionResult> Replies()
        {
            if (!IsUserAdmin())
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok!";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                using var client = CreateAuthorizedClient();
                var response = await client.GetAsync("/api/Admin/replies/all");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Yanıtlar alınamadı! Status: {response.StatusCode}";
                    return View(new List<ReplyDto>()); 
                }

                var json = await response.Content.ReadAsStringAsync();
                var replies = JsonConvert.DeserializeObject<List<ReplyDto>>(json); 

                return View(replies ?? new List<ReplyDto>()); 
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(new List<ReplyDto>()); 
            }
        }
    }
}
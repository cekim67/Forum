using ForumUi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ForumUi.Controllers
{
    public class TopicsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public TopicsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isLoggedIn = !string.IsNullOrEmpty(token);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            // Eğer giriş yapılmışsa token ekle
            if (isLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/Topics");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Konular alınamadı.";
                ViewBag.IsLoggedIn = isLoggedIn;
                return View(new List<TopicDto>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var topics = JsonConvert.DeserializeObject<List<TopicDto>>(content);

            // Çift görünme sorununu çözmek için
            var uniqueTopics = topics?.GroupBy(t => t.Id).Select(g => g.First()).ToList() ?? new List<TopicDto>();

            ViewBag.IsLoggedIn = isLoggedIn;

            // Başarı/hata mesajları için
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"].ToString();
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"].ToString();

            return View(uniqueTopics);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Konu oluşturmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTopicViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Konu oluşturmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Topics", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Konu oluşturulamadı!";
                return View(model);
            }

            TempData["Success"] = "Konu başarıyla oluşturuldu!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isLoggedIn = !string.IsNullOrEmpty(token);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            // Eğer giriş yapılmışsa token ekle
            if (isLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Konu bilgisi
            var topicResponse = await client.GetAsync($"/api/Topics/{id}");
            if (!topicResponse.IsSuccessStatusCode) return NotFound();

            var topicJson = await topicResponse.Content.ReadAsStringAsync();
            var topic = JsonConvert.DeserializeObject<TopicDto>(topicJson);

            // Yanıtlar - Cache'i bypass etmek için timestamp ekle
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var replyResponse = await client.GetAsync($"/api/Replies/topic/{id}?_t={timestamp}");
            var replies = new List<ReplyDto>();

            if (replyResponse.IsSuccessStatusCode)
            {
                var replyJson = await replyResponse.Content.ReadAsStringAsync();
                var allReplies = JsonConvert.DeserializeObject<List<ReplyDto>>(replyJson) ?? new List<ReplyDto>();

                // Yanıtları hiyerarşik olarak düzenle
                replies = OrganizeRepliesHierarchically(allReplies);
            }

            ViewBag.Replies = replies;
            ViewBag.Topic = topic;
            ViewBag.TopicId = id;
            ViewBag.IsLoggedIn = isLoggedIn;

            // Başarı/hata mesajları için
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"].ToString();
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"].ToString();

            return View(new CreateReplyViewModel { TopicId = id });
        }

        private List<ReplyDto> OrganizeRepliesHierarchically(List<ReplyDto> allReplies)
        {
            var parentReplies = allReplies.Where(r => r.ParentReplyId == null).OrderBy(r => r.CreatedAt).ToList();

            foreach (var parent in parentReplies)
            {
                parent.ChildReplies = GetChildReplies(allReplies, parent.Id);
            }

            return parentReplies;
        }

        private List<ReplyDto> GetChildReplies(List<ReplyDto> allReplies, int parentId)
        {
            var childReplies = allReplies.Where(r => r.ParentReplyId == parentId).OrderBy(r => r.CreatedAt).ToList();

            foreach (var child in childReplies)
            {
                child.ChildReplies = GetChildReplies(allReplies, child.Id);
            }

            return childReplies;
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(CreateReplyViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Yanıt yazmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Yanıt içeriği boş olamaz!";
                return RedirectToAction("Details", new { id = model.TopicId });
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Replies", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Yanıt başarıyla eklendi!";
            }
            else
            {
                TempData["Error"] = "Yanıt eklenirken hata oluştu!";
            }

            return RedirectToAction("Details", new { id = model.TopicId });
        }

        [HttpPost]
        public async Task<IActionResult> LikeReply(int replyId, int topicId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Beğenmek için giriş yapmalısınız." });
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/Likes/reply/{replyId}", null);

            if (response.IsSuccessStatusCode)
            {
                // Güncellenmiş beğeni sayısını al
                var likeResponse = await client.GetAsync($"/api/Replies/{replyId}");
                if (likeResponse.IsSuccessStatusCode)
                {
                    var likeJson = await likeResponse.Content.ReadAsStringAsync();
                    var reply = JsonConvert.DeserializeObject<ReplyDto>(likeJson);
                    return Json(new { success = true, likeCount = reply?.LikeCount ?? 0, liked = reply?.Liked ?? false });
                }
                return Json(new { success = true, likeCount = 0, liked = false });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Beğeni işlemi başarısız oldu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikeTopic(int topicId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Beğenmek için giriş yapmalısınız." });
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/Likes/topic/{topicId}", null);

            if (response.IsSuccessStatusCode)
            {
                // Güncellenmiş beğeni sayısını al
                var likeResponse = await client.GetAsync($"/api/Topics/{topicId}");
                if (likeResponse.IsSuccessStatusCode)
                {
                    var likeJson = await likeResponse.Content.ReadAsStringAsync();
                    var topic = JsonConvert.DeserializeObject<TopicDto>(likeJson);
                    return Json(new { success = true, likeCount = topic?.LikeCount ?? 0, liked = topic?.Liked ?? false });
                }
                return Json(new { success = true, likeCount = 0, liked = false });
            }
            else
            {
                return Json(new { success = false, message = "Beğeni işlemi başarısız oldu." });
            }
        }
    }
}
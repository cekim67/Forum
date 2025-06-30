using ForumUi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172"); 
            var token = HttpContext.Session.GetString("JWToken");
            var isLoggedIn = !string.IsNullOrEmpty(token);

            if (isLoggedIn)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/Topics");
            if (!response.IsSuccessStatusCode)
                return View(new List<TopicDto>());

            var json = await response.Content.ReadAsStringAsync();
            var topics = JsonConvert.DeserializeObject<List<TopicDto>>(json);
            

           

            ViewBag.IsLoggedIn = isLoggedIn;

            return View(topics ?? new List<TopicDto>());
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTopicDto dto)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Konu oluşturmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Topics", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Konu başarıyla oluşturuldu!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Konu oluşturulamadı!";
            return View(dto);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isLoggedIn = !string.IsNullOrEmpty(token);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");

            if (isLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            
            var topicResponse = await client.GetAsync($"/api/Topics/{id}");
            if (!topicResponse.IsSuccessStatusCode) return NotFound();

            var topicJson = await topicResponse.Content.ReadAsStringAsync();
            var topic = JsonConvert.DeserializeObject<TopicDto>(topicJson);

            
            var replyResponse = await client.GetAsync($"/api/Replies/topic/{id}");
            var replies = new List<ReplyDto>();

            if (replyResponse.IsSuccessStatusCode)
            {
                var replyJson = await replyResponse.Content.ReadAsStringAsync();
                var allReplies = JsonConvert.DeserializeObject<List<ReplyDto>>(replyJson) ?? new();

               
                replies = BuildReplyHierarchy(allReplies);
            }
           
            

            ViewBag.Replies = replies;
            ViewBag.Topic = topic;
            ViewBag.TopicId = id;
            ViewBag.IsLoggedIn = isLoggedIn;

            return View(new CreateReplyViewModel { TopicId = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(CreateReplyViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Yanıt vermek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Geçersiz veri gönderildi.";
                return RedirectToAction("Details", new { id = model.TopicId });
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Replies", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = model.ParentReplyId.HasValue ?
                    "Yanıtınız başarıyla eklendi!" :
                    "Yorumunuz başarıyla eklendi!";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Yanıt eklenirken hata oluştu: " + errorContent;
            }

            return RedirectToAction("Details", new { id = model.TopicId });
        }
        [HttpPost]
        public async Task<IActionResult> LikeReply(int replyId, int topicId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Beğenmek için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/Likes/reply/{replyId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Beğeni eklendi!";
            }
            else
            {
                TempData["Error"] = "Beğeni eklenirken hata oluştu!";
            }

            return RedirectToAction("Details", new { id = topicId });
        }

        
        private List<ReplyDto> BuildReplyHierarchy(List<ReplyDto> allReplies)
        {
            
            var replyDict = allReplies.ToDictionary(r => r.Id, r => r);
            var rootReplies = new List<ReplyDto>();

            
            foreach (var reply in allReplies)
            {
                if (reply.ParentReplyId == null)
                {
                    
                    rootReplies.Add(reply);
                }
                else if (replyDict.ContainsKey(reply.ParentReplyId.Value))
                {
                    
                    var parentReply = replyDict[reply.ParentReplyId.Value];
                    if (parentReply.ChildReplies == null)
                        parentReply.ChildReplies = new List<ReplyDto>();

                    parentReply.ChildReplies.Add(reply);
                }
            }

            
            SortRepliesRecursively(rootReplies);

            return rootReplies.OrderBy(r => r.CreatedAt).ToList();
        }

        
        private void SortRepliesRecursively(List<ReplyDto> replies)
        {
            foreach (var reply in replies)
            {
                if (reply.ChildReplies != null && reply.ChildReplies.Any())
                {
                    reply.ChildReplies = reply.ChildReplies.OrderBy(r => r.CreatedAt).ToList();
                    SortRepliesRecursively(reply.ChildReplies);
                }
            }
        }
        

    }
}

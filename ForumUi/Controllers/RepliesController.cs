using ForumUi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ForumUi.Controllers
{
    public class RepliesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public RepliesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReplyViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Cevap yazmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Auth");
            }

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7172");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Replies", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Yanıt başarıyla eklendi!";
            else
                TempData["Error"] = "Yanıt eklenirken hata oluştu.";

            return RedirectToAction("Details", "Topics", new { id = model.TopicId });
        }
    }
}

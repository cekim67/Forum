using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.Models.Dtos.Topic;
using ForumApi.Services.Interfaces;
using System.Security.Claims;

namespace ForumApi.Controllers
{
    /// <summary>
    /// Konu işlemleri için controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        /// <summary>
        /// TopicsController constructor
        /// </summary>
        /// <param name="topicService">Konu servisi</param>
        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        /// <summary>
        /// Tüm konuları getirir
        /// </summary>
        /// <returns>Tüm konuların listesi</returns>
        /// <response code="200">Konular başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var topics = await _topicService.GetAllAsync();
            return Ok(topics);
        }

        /// <summary>
        /// Belirli bir konuyu ID ile getirir
        /// </summary>
        /// <param name="id">Getirilecek konunun ID'si</param>
        /// <returns>Konu detay bilgileri</returns>
        /// <response code="200">Konu başarıyla döndürüldü</response>
        /// <response code="404">Konu bulunamadı</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
                return NotFound();
            return Ok(topic);
        }

        /// <summary>
        /// Yeni bir konu oluşturur
        /// </summary>
        /// <param name="dto">Konu oluşturma bilgileri</param>
        /// <returns>Oluşturulan konu bilgileri</returns>
        /// <response code="200">Konu başarıyla oluşturuldu</response>
        /// <response code="401">Kullanıcı giriş yapmamış veya geçersiz token</response>
        /// <response code="400">Geçersiz veri gönderildi</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTopicDto dto)
        {
            // Tokendan userId çekimi
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Geçersiz token.");

            var userId = int.Parse(userIdClaim.Value);
            var created = await _topicService.CreateAsync(dto, userId);
            return Ok(created);
        }

        
    }
}
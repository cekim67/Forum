using ForumApi.Models.Dtos.Reply;
using ForumApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumApi.Controllers
{
    /// <summary>
    /// Yanıt işlemleri için controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RepliesController : ControllerBase
    {
        private readonly IReplyService _replyService;

        /// <summary>
        /// RepliesController constructor
        /// </summary>
        /// <param name="replyService">Yanıt servisi</param>
        public RepliesController(IReplyService replyService)
        {
            _replyService = replyService;
        }

        /// <summary>
        /// Belirli bir konuya ait tüm yanıtları getirir
        /// </summary>
        /// <param name="topicId">Yanıtları getirilecek konunun ID'si</param>
        /// <returns>Konuya ait yanıtların listesi</returns>
        /// <response code="200">Yanıtlar başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("topic/{topicId}")]
        public async Task<IActionResult> GetRepliesByTopic(int topicId)
        {
            var replies = await _replyService.GetRepliesByTopicAsync(topicId);
            return Ok(replies);
        }

        /// <summary>
        /// Bir konuya yeni yanıt ekler
        /// </summary>
        /// <param name="dto">Yanıt oluşturma bilgileri</param>
        /// <returns>Oluşturulan yanıt bilgileri</returns>
        /// <response code="200">Yanıt başarıyla oluşturuldu</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        /// <response code="400">Geçersiz veri gönderildi</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _replyService.CreateAsync(dto, userId);
            return Ok(result);
        }

        /// <summary>
        /// Bir yanıtı siler (sadece yanıt sahibi veya admin)
        /// </summary>
        /// <param name="id">Silinecek yanıtın ID'si</param>
        /// <returns>Silme işleminin sonucu</returns>
        /// <response code="204">Yanıt başarıyla silindi</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        /// <response code="403">Bu yanıtı silme yetkiniz yok</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");
            var success = await _replyService.DeleteAsync(id, userId, isAdmin);
            if (!success) return Forbid();
            return NoContent();
        }
    }
}
using ForumApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumApi.Controllers
{
    /// <summary>
    /// Beğeni işlemleri için controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        /// <summary>
        /// LikesController constructor
        /// </summary>
        /// <param name="likeService">Beğeni servisi</param>
        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        /// <summary>
        /// Bir konuyu beğenir veya beğeniyi kaldırır
        /// </summary>
        /// <param name="topicId">Beğenilecek/beğenisi kaldırılacak konunun ID'si</param>
        /// <returns>Beğeni durumu (true: beğenildi, false: beğeni kaldırıldı)</returns>
        /// <response code="200">Beğeni işlemi başarıyla tamamlandı</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("topic/{topicId}")]
        public async Task<IActionResult> ToggleTopicLike(int topicId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _likeService.ToggleLikeTopicAsync(topicId, userId);
            return Ok(new { liked = result });
        }

        /// <summary>
        /// Bir yanıtı beğenir veya beğeniyi kaldırır
        /// </summary>
        /// <param name="replyId">Beğenilecek/beğenisi kaldırılacak yanıtın ID'si</param>
        /// <returns>Beğeni durumu (true: beğenildi, false: beğeni kaldırıldı)</returns>
        /// <response code="200">Beğeni işlemi başarıyla tamamlandı</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("reply/{replyId}")]
        public async Task<IActionResult> ToggleReplyLike(int replyId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _likeService.ToggleLikeReplyAsync(replyId, userId);
            return Ok(new { liked = result });
        }

        /// <summary>
        /// Bir konunun toplam beğeni sayısını getirir
        /// </summary>
        /// <param name="topicId">Beğeni sayısı öğrenilecek konunun ID'si</param>
        /// <returns>Konunun toplam beğeni sayısı</returns>
        /// <response code="200">Beğeni sayısı başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("topic/{topicId}/count")]
        public async Task<IActionResult> GetTopicLikeCount(int topicId)
        {
            var count = await _likeService.GetTopicLikeCountAsync(topicId);
            return Ok(new { count });
        }

        /// <summary>
        /// Bir yanıtın toplam beğeni sayısını getirir
        /// </summary>
        /// <param name="replyId">Beğeni sayısı öğrenilecek yanıtın ID'si</param>
        /// <returns>Yanıtın toplam beğeni sayısı</returns>
        /// <response code="200">Beğeni sayısı başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("reply/{replyId}/count")]
        public async Task<IActionResult> GetReplyLikeCount(int replyId)
        {
            var count = await _likeService.GetReplyLikeCountAsync(replyId);
            return Ok(new { count });
        }

        /// <summary>
        /// Kullanıcının beğendiği konuları getirir
        /// </summary>
        /// <returns>Kullanıcının beğendiği konuların listesi</returns>
        /// <response code="200">Beğenilen konular başarıyla döndürüldü</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("topics")]
        public async Task<IActionResult> GetLikedTopics()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var topics = await _likeService.GetLikedTopicsAsync(userId);
            return Ok(topics);
        }

        /// <summary>
        /// Kullanıcının beğendiği yanıtları getirir
        /// </summary>
        /// <returns>Kullanıcının beğendiği yanıtların listesi</returns>
        /// <response code="200">Beğenilen yanıtlar başarıyla döndürüldü</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("replies")]
        public async Task<IActionResult> GetLikedReplies()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var replies = await _likeService.GetLikedRepliesAsync(userId);
            return Ok(replies);
        }
    }
}
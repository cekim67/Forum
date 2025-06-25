using ForumApi.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApiContext _context;

        public AdminController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sistemdeki tüm kullanıcıları getirir
        /// </summary>
        /// <returns>Kullanıcıların temel bilgilerini içeren liste</returns>
        /// <response code="200">Kullanıcı listesi başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsAdmin,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Belirtilen kullanıcıyı admin yapar
        /// </summary>
        /// <param name="id">Admin yapılacak kullanıcının ID'si</param>
        /// <returns>Güncellenmiş kullanıcı bilgileri</returns>
        /// <response code="200">Kullanıcı başarıyla admin yapıldı</response>
        /// <response code="404">Kullanıcı bulunamadı</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("users/{id}/make-admin")]
        public async Task<IActionResult> MakeUserAdmin(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsAdmin = true;
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, IsAdmin = user.IsAdmin });
        }

        /// <summary>
        /// Kullanıcının aktif/pasif durumunu değiştirir
        /// </summary>
        /// <param name="id">Durumu değiştirilecek kullanıcının ID'si</param>
        /// <returns>Güncellenmiş kullanıcı durumu</returns>
        /// <response code="200">Kullanıcı durumu başarıyla değiştirildi</response>
        /// <response code="400">Admin kullanıcılar pasif hale getirilemez</response>
        /// <response code="404">Kullanıcı bulunamadı</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (user.IsAdmin)
                return BadRequest("Admin kullanıcılar pasif hale getirilemez!");

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.IsActive });
        }

        /// <summary>
        /// Forumdaki tüm konuları getirir
        /// </summary>
        /// <returns>Yazar bilgileri ile birlikte tüm konuların listesi</returns>
        /// <response code="200">Konu listesi başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("topics/all")]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _context.Topics
                .Include(t => t.User)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    t.IsDeleted,
                    Username = t.User.Username,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(topics);
        }

        /// <summary>
        /// Forumdaki tüm yanıtları getirir
        /// </summary>
        /// <returns>Yazar ve konu bilgileri ile birlikte tüm yanıtların listesi</returns>
        /// <response code="200">Yanıt listesi başarıyla döndürüldü</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("replies/all")]
        public async Task<IActionResult> GetAllReplies()
        {
            var replies = await _context.Replies
                .Include(r => r.User)
                .Include(r => r.Topic)
                .Select(r => new
                {
                    r.Id,
                    r.Content,
                    r.TopicId,
                    r.IsDeleted,
                    Username = r.User.Username,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(replies);
        }

        /// <summary>
        /// Forum istatistiklerini getirir
        /// </summary>
        /// <returns>Forum platformunun genel istatistik verilerini içeren rapor</returns>
        /// <response code="200">İstatistik verileri başarıyla döndürüldü</response>
        /// <remarks>
        /// Bu endpoint aşağıdaki istatistikleri döndürür:
        /// - Toplam kullanıcı sayısı
        /// - Aktif kullanıcı sayısı
        /// - Admin sayısı
        /// - Toplam konu sayısı
        /// - Toplam yanıt sayısı
        /// - En çok beğenilen konu bilgisi
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var adminCount = await _context.Users.CountAsync(u => u.IsAdmin);
            var totalTopics = await _context.Topics.CountAsync();
            var totalReplies = await _context.Replies.CountAsync();

            var mostLikedTopic = await _context.Topics
                .Include(t => t.Likes)
                .OrderByDescending(t => t.Likes.Count)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    LikeCount = t.Likes.Count
                })
                .FirstOrDefaultAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                AdminCount = adminCount,
                TotalTopics = totalTopics,
                TotalReplies = totalReplies,
                MostLikedTopic = mostLikedTopic
            });
        }

    }
}
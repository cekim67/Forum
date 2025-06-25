using ForumApi.Models.Dtos.User;
using ForumApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumApi.Controllers
{
    /// <summary>
    /// Kullanıcı işlemleri için controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// UsersController constructor
        /// </summary>
        /// <param name="userService">Kullanıcı servisi</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Giriş yapmış kullanıcının profil bilgilerini getirir
        /// </summary>
        /// <returns>Kullanıcının profil bilgileri</returns>
        /// <response code="200">Profil bilgileri başarıyla döndürüldü</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        /// <response code="404">Kullanıcı bulunamadı</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetMeAsync(userId);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Giriş yapmış kullanıcının oluşturduğu konuları getirir
        /// </summary>
        /// <returns>Kullanıcının oluşturduğu konuların listesi</returns>
        /// <response code="200">Konular başarıyla döndürüldü</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("me/topics")]
        public async Task<IActionResult> GetMyTopics()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var topics = await _userService.GetUserTopicsAsync(userId);
            return Ok(topics);
        }

        /// <summary>
        /// Kullanıcının profil bilgilerini günceller
        /// </summary>
        /// <param name="dto">Güncellenecek profil bilgileri</param>
        /// <returns>Güncelleme işleminin sonucu</returns>
        /// <response code="200">Profil başarıyla güncellendi</response>
        /// <response code="400">Profil güncellenemedi veya geçersiz veri</response>
        /// <response code="401">Kullanıcı giriş yapmamış</response>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _userService.UpdateProfileAsync(userId, dto);
            if (!result) return BadRequest("Profil güncellenemedi.");
            return Ok("Profil güncellendi.");
        }
    }
}
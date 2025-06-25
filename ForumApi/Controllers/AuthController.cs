using ForumApi.Models.Dtos.Auth;
using ForumApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    /// <summary>
    /// Kimlik doğrulama işlemleri için controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// AuthController constructor
        /// </summary>
        /// <param name="authService">Kimlik doğrulama servisi</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Yeni bir kullanıcı kaydı oluşturur
        /// </summary>
        /// <param name="dto">Kullanıcı kayıt bilgileri</param>
        /// <returns>Başarılı kayıt durumunda JWT token döner</returns>
        /// <response code="200">Kullanıcı başarıyla kaydedildi ve token döndürüldü</response>
        /// <response code="400">Email adresi zaten kullanılıyor veya geçersiz veri</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);
            if (token == null)
                return BadRequest("Bu email adresi zaten kullanılıyor.");

            return Ok(new { token });
        }

        /// <summary>
        /// Kullanıcı giriş işlemi yapar
        /// </summary>
        /// <param name="dto">Kullanıcı giriş bilgileri (email ve şifre)</param>
        /// <returns>Başarılı giriş durumunda JWT token döner</returns>
        /// <response code="200">Giriş başarılı, token döndürüldü</response>
        /// <response code="401">Geçersiz kullanıcı adı veya şifre</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");

            return Ok(new { token });
        }
    }
}
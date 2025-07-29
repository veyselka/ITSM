using Havsan.NovaDesk.API.Models;
using Havsan.NovaDesk.API.Models.DTOs; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims; 
using System.Text; 

namespace Havsan.NovaDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KimlikDogrulamaKontrolcusu : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration; 

        
        public KimlikDogrulamaKontrolcusu(UserManager<IdentityUser> userManager,
                                      SignInManager<IdentityUser> signInManager,
                                      IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        
        [HttpPost("Kayit")] 
        public async Task<IActionResult> Kayit([FromBody] KayitModeli model)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            
            var kullanici = new IdentityUser
            {
                UserName = model.KullaniciAdi,
                Email = model.Eposta
            };

            
            var sonuc = await _userManager.CreateAsync(kullanici, model.Sifre);

            if (sonuc.Succeeded)
            {

                var roleResult = await _userManager.AddToRoleAsync(kullanici, KullaniciRolleri.SonKullanici);

                if (!roleResult.Succeeded)
                {
                    // Rol ataması başarısız olursa (nadiren olur)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Kullanıcı oluşturuldu ancak varsayılan rol atanamadı.", Errors = roleResult.Errors.Select(e => e.Description) });
                }

                return Ok(new { Message = "Kullanıcı başarıyla kaydedildi!" });
            }

            
            return BadRequest(new { Errors = sonuc.Errors.Select(e => e.Description) });
        }

        
        [HttpPost("Giris")] 
        public async Task<IActionResult> Giris([FromBody] GirisModeli model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var sonuc = await _signInManager.PasswordSignInAsync(model.KullaniciAdi, model.Sifre, false, false);
            

            if (sonuc.Succeeded)
            {
                
                var kullanici = await _userManager.FindByNameAsync(model.KullaniciAdi);
                if (kullanici == null)
                {
                    return Unauthorized(new { Message = "Kullanıcı bulunamadı." }); 
                }

                var kullaniciRolleri = await _userManager.GetRolesAsync(kullanici);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Benzersiz token ID'si
                };

                foreach (var kullaniciRol in kullaniciRolleri)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, kullaniciRol));
                }

                // JWT Secret, Issuer ve Audience değerlerini appsettings.json'dan al
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)); // Uyarıyı giderdik!

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3), // Token 3 saat sonra geçerliliğini yitirecek
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Token'ı ve geçerlilik süresini döndür
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    GecerlilikSuresi = token.ValidTo // Token'ın geçerli olacağı tarih ve saat
                });
            }

            // Giriş başarısızsa
            return Unauthorized(new { Message = "Geçersiz kullanıcı adı veya şifre." });
        }
    }
}
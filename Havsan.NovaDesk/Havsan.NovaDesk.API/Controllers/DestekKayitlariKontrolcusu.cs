using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Havsan.NovaDesk.API.Data;
using Havsan.NovaDesk.API.Models;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 

namespace Havsan.NovaDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class DestekKayitlariKontrolcusu : ControllerBase
    {
        private readonly UygulamaVeriBaglami _veriBaglami;

        public DestekKayitlariKontrolcusu(UygulamaVeriBaglami veriBaglami)
        {
            _veriBaglami = veriBaglami;
        }

       
        [HttpGet]
        [Authorize(Roles = KullaniciRolleri.Admin + "," + KullaniciRolleri.DestekPersoneli)]
        public async Task<ActionResult<IEnumerable<DestekKaydi>>> KayitlariGetir()
        {
            return await _veriBaglami.DestekKayitlari.ToListAsync();
        }

        
        [HttpGet("{id}")]
        [Authorize(Roles = KullaniciRolleri.Admin + "," + KullaniciRolleri.DestekPersoneli + "," + KullaniciRolleri.SonKullanici)]
        public async Task<ActionResult<DestekKaydi>> KayitGetir(int id)
        {
            var destekKaydi = await _veriBaglami.DestekKayitlari.FindAsync(id);

            if (destekKaydi == null)
            {
                return NotFound();
            }

           
             
             if (User.IsInRole(KullaniciRolleri.SonKullanici) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != destekKaydi.OlusturanKullaniciId)
             {
                 return Forbid(); // 403 Forbidden
             }

            return destekKaydi;
        }

        
        [HttpPost]
        [Authorize(Roles = KullaniciRolleri.Admin + "," + KullaniciRolleri.DestekPersoneli + "," + KullaniciRolleri.SonKullanici)]
        public async Task<ActionResult<DestekKaydi>> KayitEkle(DestekKaydi kayit)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                kayit.OlusturanKullaniciId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            _veriBaglami.DestekKayitlari.Add(kayit);
            await _veriBaglami.SaveChangesAsync();

            return CreatedAtAction("KayitGetir", new { id = kayit.Id }, kayit);
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = KullaniciRolleri.Admin + "," + KullaniciRolleri.DestekPersoneli)]
        public async Task<IActionResult> KayitGuncelle(int id, DestekKaydi kayit)
        {
            if (id != kayit.Id)
            {
                return BadRequest();
            }

            _veriBaglami.Entry(kayit).State = EntityState.Modified;

            try
            {
                await _veriBaglami.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestekKaydiVarMi(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = KullaniciRolleri.Admin)]
        public async Task<IActionResult> KayitSil(int id)
        {
            var destekKaydi = await _veriBaglami.DestekKayitlari.FindAsync(id);
            if (destekKaydi == null)
            {
                return NotFound();
            }

            _veriBaglami.DestekKayitlari.Remove(destekKaydi);
            await _veriBaglami.SaveChangesAsync();

            return NoContent();
        }

        private bool DestekKaydiVarMi(int id)
        {
            return _veriBaglami.DestekKayitlari.Any(e => e.Id == id);
        }
    }
}
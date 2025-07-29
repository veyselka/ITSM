using System.ComponentModel.DataAnnotations;

namespace Havsan.NovaDesk.API.Models.DTOs
{
    public class GirisModeli
    {
        [Required(ErrorMessage = "Kullanıcı Adı gereklidir.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Sifre { get; set; } = string.Empty;
    }
}
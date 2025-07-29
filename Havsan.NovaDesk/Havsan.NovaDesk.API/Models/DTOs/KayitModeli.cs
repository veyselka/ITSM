using System.ComponentModel.DataAnnotations; 

namespace Havsan.NovaDesk.API.Models.DTOs
{
    public class KayitModeli
    {
        [Required(ErrorMessage = "Kullanıcı Adı gereklidir.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")] 
        public string Sifre { get; set; } = string.Empty;
    }
}
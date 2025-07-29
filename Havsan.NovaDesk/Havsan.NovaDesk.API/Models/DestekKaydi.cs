namespace Havsan.NovaDesk.API.Models
{
    public class DestekKaydi
    {
        public int Id { get; set; } 
        public string Konu { get; set; } = string.Empty; 
        public string Aciklama { get; set; } = string.Empty; 
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now; 
        public string Durum { get; set; } = "Yeni"; 
        public string Oncelik { get; set; } = "Orta"; 
        public string Kategori { get; set; } = "Genel"; 
        public string? AtananKisi { get; set; }

        public string? OlusturanKullaniciId { get; set; }
    }
}

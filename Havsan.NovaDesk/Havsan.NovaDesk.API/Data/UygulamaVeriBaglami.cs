using Microsoft.EntityFrameworkCore;
using Havsan.NovaDesk.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Bu using ifadesini ekleyin!
using Microsoft.AspNetCore.Identity; // IdentityUser sınıfı için (eğer kullanacaksak)

namespace Havsan.NovaDesk.API.Data
{
    // DbContext yerine IdentityDbContext'ten miras alıyoruz.
    // IdentityUser, varsayılan kullanıcı sınıfıdır. İsterseniz kendi özel Kullanici sınıfınızı da oluşturabilirsiniz.
    public class UygulamaVeriBaglami : IdentityDbContext<IdentityUser>
    {
        public UygulamaVeriBaglami(DbContextOptions<UygulamaVeriBaglami> options)
            : base(options)
        {
        }

        public DbSet<DestekKaydi> DestekKayitlari { get; set; }

        // IdentityDbContext'i kullandığımızda, OnModelCreating metodunu override etmek iyi bir pratiktir.
        // Bu, Identity tablolarının doğru şekilde oluşturulmasını sağlar.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Burada Identity tabloları için ek yapılandırmalar yapabilirsiniz,
            // örneğin tablo isimlerini değiştirmek gibi. Şimdilik varsayılanı kullanacağız.
        }
    }
}
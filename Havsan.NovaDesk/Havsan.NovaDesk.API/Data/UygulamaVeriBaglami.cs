using Microsoft.EntityFrameworkCore;
using Havsan.NovaDesk.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.AspNetCore.Identity; 

namespace Havsan.NovaDesk.API.Data
{
    
    public class UygulamaVeriBaglami : IdentityDbContext<IdentityUser>
    {
        public UygulamaVeriBaglami(DbContextOptions<UygulamaVeriBaglami> options)
            : base(options)
        {
        }

        public DbSet<DestekKaydi> DestekKayitlari { get; set; }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }
    }
}
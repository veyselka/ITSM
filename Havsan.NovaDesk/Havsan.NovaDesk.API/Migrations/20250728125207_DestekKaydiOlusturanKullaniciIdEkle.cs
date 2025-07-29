using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havsan.NovaDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class DestekKaydiOlusturanKullaniciIdEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OlusturanKullaniciId",
                table: "DestekKayitlari",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OlusturanKullaniciId",
                table: "DestekKayitlari");
        }
    }
}

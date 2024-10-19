
namespace QLNH.Data.ViewModels
{
    public class DonHangViewModel
    {
        public int DonHangId { get; set; }
        public int? BanId { get; set; }
        public int? SoBan { get; set; }
        public int? NhanVienId { get; set; }
        public DateTime? NgayDat { get; set; } // Dùng DateTime thay vì DateOnly
        public string? TrangThai { get; set; }
        public int? KhachHangId { get; set; }
        public string? HoTen { get; set; }
    }
}

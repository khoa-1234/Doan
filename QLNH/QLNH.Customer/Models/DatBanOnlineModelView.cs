namespace QLNH.Customer.Models
{
    public class DatBanOnlineModelView
    {

        public string? HoTen { get; set; }
        public string? OTP { get; set; }
        public DateTime? ThoiGianDat { get; set; }

        public int? SoNguoi { get; set; }

        public string? GhiChu { get; set; }

        public string? PhuongThucDat { get; set; }

        public string? Email { get; set; }

        public string? SoDienThoai { get; set; }

        public DateOnly? NgayDat { get; set; }

        public int? NhanVienMoBanId { get; set; }
        public int? KhuvucId { get; set; }

        public bool? CoDatMon { get; set; }
        public List<ChiTietDonHangModelView>? MonAnDat { get; set; }
    }
}

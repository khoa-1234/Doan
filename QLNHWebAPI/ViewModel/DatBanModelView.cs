using QLNHWebAPI.Models;

namespace QLNHWebAPI.ViewModel
{
    public class DatBanModelView
    {
        public int? DatBanId { get; set; }

        public int? KhachHangId { get; set; }
        public int? KhuvucId { get; set; }

        public int? BanId { get; set; }

        public DateOnly? NgayDat { get; set; }

        public DateTime? ThoiGianDat { get; set; }

        public int? SoNguoi { get; set; }

        public string? GhiChu { get; set; }

        public string? PhuongThucDat { get; set; }

        public string? TrangThaiXacNhan { get; set; }

        public int? NhanVienMoBanId { get; set; }

        public DateTime? ThoiGianDongBan { get; set; }

        public DateTime? ThoiGianCapNhat { get; set; }

        public bool? CoDatMon { get; set; }

        public string? TrangThai { get; set; }
        public DateTime? ThoiGianTaoBan { get; set; }

        public List<MonAnModalView> MonAnDat { get; set; }



    }
}

namespace QLNH.Admin.Models
{
    public class DonHangModelView
    {
        public int? DonHangId { get; set; }

        public int? DatBanId { get; set; }

        public int? NhanVienId { get; set; }

        public DateOnly? NgayDat { get; set; }
        public int? KhachHangId { get; set; }
        public decimal? TongTien { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public List<ChiTietDonHangModelView>? ChiTietDonHangs { get; set; } = new List<ChiTietDonHangModelView>();

       
    }
}

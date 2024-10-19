namespace QLNH.Customer.Models
{
    public class ChiTietDonHangModelView
    {
        public int? ChiTietDonHangId { get; set; }

        public int? DonHangId { get; set; }

        public int? MonAnId { get; set; }

        public int? SoLuong { get; set; }

        public decimal? Gia { get; set; }

        public string? TrangThai { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        public virtual DonHangModelView? DonHang { get; set; }

        public virtual MonAnViewModel? MonAn { get; set; }
    }
}

namespace QLNH.Admin.ViewModels
{
    public class OrderDetailViewModel
    {
        public int ChiTietDonHangId { get; set; }
        public int DonHangId { get; set; }
        public int MonAnId { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string TrangThai { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string DonHang { get; set; }
        public string MonAn { get; set; }
    }
}

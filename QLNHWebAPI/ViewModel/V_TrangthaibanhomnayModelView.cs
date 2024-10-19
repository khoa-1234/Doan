namespace QLNHWebAPI.ViewModel
{
    public class V_TrangthaibanhomnayModelView
    {
        public int? BanId { get; set; }

        public int? KhuVucId { get; set; }

        public string? DonHangId { get; set; } = null!;
        public string TrangThai { get; set; } = null!;

        public string NgayGioDat { get; set; } = null!;

        public string KhachHangId { get; set; } = null!;

        public string? DatBanId { get; set; } = null!;
        public BanModelView Ban { get; set; }
        public KhuVucModelView KhuVuc { get; set; }
    }
}

namespace QLNH.Customer.Models
{
    public class KhuVucModelView
    {
        public int KhuVucId { get; set; }

        public string TenKhuVuc { get; set; } = null!;

        public string? Mota { get; set; }

        public DateTime? ThoiGianTao { get; set; }

        public DateTime? ThoiGianCapNhat { get; set; }
    }
}

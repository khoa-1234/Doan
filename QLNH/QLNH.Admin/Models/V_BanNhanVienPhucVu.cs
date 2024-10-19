using QLNH.Data.Models;

public class V_BanNhanVienPhucVu
{
    public int? BanId { get; set; }
    public int? SoBan { get; set; }
    public int? KhuVucId { get; set; }
    public int? SoGhe { get; set; }
    public string? DonHangId { get; set; } = null!;

    public string TrangThai { get; set; } = null!;
    public string NgayGioDat { get; set; } = null!;
    public string KhachHangId { get; set; } = null!;
    public string DatBanId { get; set; } = null!;

    public string? LoaiBan { get; set; }
    public Ban Ban { get; set; }
    public KhuVuc KhuVuc { get; set; }
}

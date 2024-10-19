using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class NhanVien
{
    public int NhanVienId { get; set; }

    public string? HoTen { get; set; }

    public string? ChucVu { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? BoPhan { get; set; }

    public string? HinhAnhDaiDien { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<DangKyLichLamViec> DangKyLichLamViecs { get; set; } = new List<DangKyLichLamViec>();

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();

    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    public virtual Puser? User { get; set; }

    public virtual ICollection<VanTay> VanTays { get; set; } = new List<VanTay>();
}

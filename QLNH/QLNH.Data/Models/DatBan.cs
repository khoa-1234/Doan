using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class DatBan
{
    public int DatBanId { get; set; }

    public int? KhachHangId { get; set; }

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

    public int? KhuVucId { get; set; }

    public string? MaQr { get; set; }

    public virtual Ban? Ban { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual KhuVuc? KhuVuc { get; set; }

    public virtual NhanVien? NhanVienMoBan { get; set; }
}


using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class DonHang
{
    public int DonHangId { get; set; }

    public int? DatBanId { get; set; }

    public int? NhanVienId { get; set; }

    public DateOnly? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public string? TrangThai { get; set; }

    public int? KhachHangId { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhachHang? KhachHang { get; set; }

    public virtual NhanVien? NhanVien { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

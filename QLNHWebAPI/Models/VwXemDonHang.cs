using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class VwXemDonHang
{
    public int DonHangId { get; set; }

    public int? BanId { get; set; }

    public int? SoBan { get; set; }

    public int? NhanVienId { get; set; }

    public DateOnly? NgayDat { get; set; }

    public string? TrangThai { get; set; }

    public int? KhachHangId { get; set; }

    public string? HoTen { get; set; }
}

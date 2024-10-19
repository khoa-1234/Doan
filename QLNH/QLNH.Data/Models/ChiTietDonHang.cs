using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class ChiTietDonHang
{
    public int ChiTietDonHangId { get; set; }

    public int? DonHangId { get; set; }

    public int? SoLuong { get; set; }

    public decimal? Gia { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public int? ChiTietMonAnId { get; set; }

    public int? MonAnId { get; set; }

    public virtual DonHang? DonHang { get; set; }

    public virtual MonAn? MonAn { get; set; }
}

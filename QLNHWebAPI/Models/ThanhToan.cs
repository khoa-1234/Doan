using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class ThanhToan
{
    public int ThanhToanId { get; set; }

    public int? DonHangId { get; set; }

    public string? MaQr { get; set; }

    public decimal? SoTien { get; set; }

    public DateTime? ThoiGianThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public virtual DonHang? DonHang { get; set; }
}

using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class HoaDon
{
    public int HoaDonId { get; set; }

    public int? DonHangId { get; set; }

    public DateOnly? NgayHoaDon { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? Vat { get; set; }

    public decimal? TongTienSauVat { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public virtual DonHang? DonHang { get; set; }
}

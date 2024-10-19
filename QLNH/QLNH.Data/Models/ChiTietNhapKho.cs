using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class ChiTietNhapKho
{
    public int ChiTietNhapKhoId { get; set; }

    public int? VatTuId { get; set; }

    public DateOnly? NgayNhap { get; set; }

    public int? SoLuong { get; set; }

    public decimal? GiaNhap { get; set; }

    public virtual VatTu? VatTu { get; set; }
}

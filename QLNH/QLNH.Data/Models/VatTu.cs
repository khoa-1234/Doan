using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class VatTu
{
    public int VatTuId { get; set; }

    public string? TenVatTu { get; set; }

    public int? SoLuong { get; set; }

    public string? DonViTinh { get; set; }

    public decimal? GiaMua { get; set; }

    public virtual ICollection<ChiTietNhapKho> ChiTietNhapKhoes { get; set; } = new List<ChiTietNhapKho>();
}

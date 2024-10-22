using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class TienCoc
{
    public int TienCocId { get; set; }

    public int DatBanId { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public decimal SoTien { get; set; }

    public string? MaGiaoDich { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public virtual DatBan DatBan { get; set; } = null!;
}

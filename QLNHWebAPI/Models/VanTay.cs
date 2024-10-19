using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class VanTay
{
    public int VanTayId { get; set; }

    public int? NhanVienId { get; set; }

    public string? MaVanTayHex { get; set; }

    public string? MoTa { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public virtual NhanVien? NhanVien { get; set; }
}

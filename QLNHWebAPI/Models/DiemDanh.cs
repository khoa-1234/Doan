using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class DiemDanh
{
    public int DiemDanhId { get; set; }

    public int? NhanVienId { get; set; }

    public string MaVanTayHex { get; set; } = null!;

    public DateOnly? Ngay { get; set; }

    public TimeOnly? ThoiGianVao { get; set; }

    public TimeOnly? ThoiGianRa { get; set; }

    public int? VanTayId { get; set; }

    public virtual NhanVien? NhanVien { get; set; }

    public virtual VanTay? VanTay { get; set; }
}

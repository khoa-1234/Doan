using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class DangKyLichLamViec
{
    public int DangKyLichLamViecId { get; set; }

    public int NhanVienId { get; set; }

    public DateOnly NgayLamViec { get; set; }

    public TimeOnly ThoiGianBatDau { get; set; }

    public TimeOnly ThoiGianKetThuc { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    public virtual NhanVien NhanVien { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class LichLamViec
{
    public int LichLamViecId { get; set; }

    public int NhanVienId { get; set; }

    public DateOnly NgayLamViec { get; set; }

    public TimeOnly ThoiGianBatDau { get; set; }

    public TimeOnly ThoiGianKetThuc { get; set; }

    public int? DangKyLichLamViecId { get; set; }

    public string? GhiChu { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public virtual DangKyLichLamViec? DangKyLichLamViec { get; set; }

    public virtual NhanVien NhanVien { get; set; } = null!;
}

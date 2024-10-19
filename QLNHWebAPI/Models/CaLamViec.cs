using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class CaLamViec
{
    public int CaLamViecId { get; set; }

    public string? TenCa { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }
}

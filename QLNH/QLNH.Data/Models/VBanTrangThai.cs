using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class VBanTrangThai
{
    public int BanId { get; set; }

    public int? KhuVucId { get; set; }

    public string TrangThai { get; set; } = null!;

    public string NgayGioDat { get; set; } = null!;

    public string? KhachHangId { get; set; }

    public string? DatBanId { get; set; }

    public string? DonHangId { get; set; }
}

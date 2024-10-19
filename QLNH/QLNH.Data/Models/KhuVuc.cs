using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class KhuVuc
{
    public int KhuVucId { get; set; }

    public string TenKhuVuc { get; set; } = null!;

    public string? Mota { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public virtual ICollection<Ban> Bans { get; set; } = new List<Ban>();

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
}

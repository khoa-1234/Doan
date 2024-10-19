using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class Ban
{
    public int BanId { get; set; }

    public int? SoBan { get; set; }

    public int? SoGhe { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public int? KhuVucId { get; set; }

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();

    public virtual KhuVuc? KhuVuc { get; set; }
}

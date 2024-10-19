using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class NhomMonAn
{
    public int NhomMonAnId { get; set; }

    public string? TenNhom { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<NhomMonAn> InverseParent { get; set; } = new List<NhomMonAn>();

    public virtual ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();

    public virtual NhomMonAn? Parent { get; set; }
}

using System;
using System.Collections.Generic;

namespace QLNHWebAPI.Models;

public partial class QuyTrinhNauAn
{
    public int QuyTrinhNauAnId { get; set; }

    public int? SanPhamId { get; set; }

    public string? MoTa { get; set; }

    public virtual MonAn? SanPham { get; set; }
}

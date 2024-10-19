using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class MonAnHinhAnh
{
    public int HinhAnhId { get; set; }

    public int MonAnId { get; set; }

    public string? HinhAnh { get; set; }

    public virtual MonAn MonAn { get; set; } = null!;
}

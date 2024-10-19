using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class ChiTietKhuyenMai
{
    public int ChiTietKhuyenMaiId { get; set; }

    public int? KhuyenMaiId { get; set; }

    public int? SanPhamId { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public virtual KhuyenMai? KhuyenMai { get; set; }

    public virtual MonAn? SanPham { get; set; }
}

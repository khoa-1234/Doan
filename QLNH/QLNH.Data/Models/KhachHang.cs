using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public string? HoTen { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual Puser? User { get; set; }
}

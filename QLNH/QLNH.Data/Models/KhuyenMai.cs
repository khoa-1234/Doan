using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;

public partial class KhuyenMai
{
    public int KhuyenMaiId { get; set; }

    public string? TenKhuyenMai { get; set; }

    public string? MoTa { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public decimal? PhanTramGiamGia { get; set; }

    public virtual ICollection<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; } = new List<ChiTietKhuyenMai>();
}

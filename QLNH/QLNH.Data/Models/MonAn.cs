using System;
using System.Collections.Generic;

namespace QLNH.Data.Models;
public partial class MonAn
{
    public int MonAnId { get; set; }

    public string? TenMonAn { get; set; }

    public string? MoTa { get; set; }

    public int? NhomMonAnId { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public decimal? Gia { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; } = new List<ChiTietKhuyenMai>();

    public virtual ICollection<MonAnHinhAnh> MonAnHinhAnhs { get; set; } = new List<MonAnHinhAnh>();

    public virtual NhomMonAn? NhomMonAn { get; set; }

    public virtual ICollection<QuyTrinhNauAn> QuyTrinhNauAns { get; set; } = new List<QuyTrinhNauAn>();
}

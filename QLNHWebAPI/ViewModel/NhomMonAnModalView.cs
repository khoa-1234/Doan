using QLNHWebAPI.Models;

namespace QLNHWebAPI.ViewModel
{
    public class NhomMonAnModalView
    {


        public int? NhomMonAnId { get; set; }

        public string? TenNhom { get; set; }

        public DateTime? ThoiGianCapNhat { get; set; }

        public DateTime? ThoiGianTao { get; set; }

        public virtual ICollection<MonAn>? MonAns { get; set; } = new List<MonAn>();




    }
}

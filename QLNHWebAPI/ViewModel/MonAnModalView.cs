using QLNHWebAPI.Models;

namespace QLNHWebAPI.ViewModel
{
    public class MonAnModalView
    {
        public int? MonAnId { get; set; }
        public string? TenMonAn { get; set; }

        public string? MoTa { get; set; }

        public decimal? Gia { get; set; }

        public int? NhomMonAnId { get; set; }

 

        public DateTime? ThoiGianCapNhat { get; set; }

        public DateTime? ThoiGianTao { get; set; }
        public IFormFile HinhAnhDaiDien { get; set; }


    }
}

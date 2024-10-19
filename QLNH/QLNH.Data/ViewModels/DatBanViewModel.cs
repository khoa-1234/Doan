using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNH.Data.ViewModels
{
    public class DatBanViewModel
    {
        public int DatBanId { get; set; }
        public DateTime ThoiGianDat { get; set; }
        public string TenKhachHang { get; set; }
        public int SoNguoi { get; set; }
        public string GhiChu { get; set; }
    }
}

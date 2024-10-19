namespace QLNH.Customer.Models
{
    public class MonAnViewModel
    {
        public int MonAnId { get; set; }
        public string TenMonAn { get; set; }
        public int NhomMonAnId { get; set; } // Đảm bảo thuộc tính này tồn tại
        public string MoTa { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh { get; set; }
    }
}

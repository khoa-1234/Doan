namespace QLNH.Customer.Models
{
    public class NhomMonAnViewModel
    {
        public int NhomMonAnId { get; set; }
        public string TenNhom { get; set; }
        public List<NhomMonAnViewModel> Children { get; set; } = new List<NhomMonAnViewModel>();
    }
}

namespace QLNH.Admin.ViewModels
{
    public class NhomMonAnModel
    {
        public int NhomMonAnId { get; set; }
        public string TenNhom { get; set; }
        public List<NhomMonAnModel> Children { get; set; } = new List<NhomMonAnModel>();
    }
}

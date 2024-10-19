namespace QLNHWebAPI.ViewModel
{
    public class PaymentRequestDto
    {
        public int DonHangId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
    }
}

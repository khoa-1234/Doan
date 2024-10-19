using Microsoft.Extensions.Configuration.UserSecrets;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace QLNHWebAPI.VNPay
{
    public class VnPayService
    {
        private readonly string _endpoint = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL môi trường TEST
        private readonly string _merchantCode = "PM25WDZ1"; // Mã website
        private readonly string _secretKey = "Z7532ZK2V2FAUU2CE0GV0151JQIBPG5C"; // Chuỗi bí mật tạo checksum

        public async Task<string> CreatePaymentRequest(string orderId, decimal amount, string orderInfo)
        {
            var returnUrl = "https://localhost:7244/api/payment/vnpay_return"; // URL trả về
            var notifyUrl = "https://localhost:7244/api/payment/vnpay_notify"; // URL thông báo

            var parameters = new Dictionary<string, string>
            {
                { "vnp_TmnCode", _merchantCode },
                { "vnp_Amount", ((int)(amount * 100)).ToString() }, // Chuyển đổi sang đơn vị đồng
                { "vnp_OrderId", orderId },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_NotifyUrl", notifyUrl },
                { "vnp_PayDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_CurrCode", "VND" }
            };

            // Tạo mã checksum
            var secret = parameters.OrderBy(x => x.Key)
                .Aggregate("", (current, param) => current + $"{param.Key}={param.Value}&").TrimEnd('&');

            var checksum = HashHelper.CreateHash(secret, _secretKey); // Sử dụng _secretKey để tạo checksum
            parameters.Add("vnp_SecureHash", checksum);

            // Tạo URL yêu cầu thanh toán
            var paymentUrl = $"{_endpoint}?{string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"))}";

            return paymentUrl; // Trả về URL thanh toán
        }

        public static class HashHelper
        {
            public static string CreateHash(string data, string key)
            {
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                    return BitConverter.ToString(hash).Replace("-", "").ToUpper();
                }
            }
        }

        private string GenerateSignature(SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            foreach (var param in parameters)
            {
                sb.Append($"{param.Key}={param.Value}&");
            }
            string queryString = sb.ToString().TrimEnd('&');

            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

        public bool ValidateSignature(SortedDictionary<string, string> receivedParameters)
        {
            if (!receivedParameters.ContainsKey("vnp_SecureHash"))
                return false;

            string receivedSignature = receivedParameters["vnp_SecureHash"];
            receivedParameters.Remove("vnp_SecureHash");

            string generatedSignature = GenerateSignature(receivedParameters);

            return string.Equals(receivedSignature, generatedSignature, StringComparison.OrdinalIgnoreCase);
        }
    }
}

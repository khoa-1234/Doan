using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace QLNHWebAPI.Momo
{
    public class MoMoService
    {
        private readonly string _endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
        private readonly string _partnerCode = "MOMOIQA420180417";
        private readonly string _accessKey = "F8BBA842ECF85";
        private readonly string _secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

        public async Task<RestResponse> CreatePaymentRequest(string orderId, decimal amount, string orderInfo, string returnUrl, string notifyUrl)
        {
            var requestBody = new
            {
                partnerCode = _partnerCode,
                accessKey = _accessKey,
                requestId = Guid.NewGuid().ToString(),
                amount = amount.ToString(),
                orderId = orderId,
                orderInfo = orderInfo,
                returnUrl = returnUrl,
                notifyUrl = notifyUrl,
                requestType = "captureMoMoWallet",
                extraData = "",
                signature = GenerateSignature(orderId, amount)
            };

            var client = new RestClient(_endpoint);
            var request = new RestRequest
            {
                Method = Method.Post
            };
            request.AddJsonBody(requestBody);

            RestResponse response = await client.ExecuteAsync(request);
            return response;
        }

        private string GenerateSignature(string orderId, decimal amount)
        {
            string rawHash = $"partnerCode={_partnerCode}&accessKey={_accessKey}&requestId={Guid.NewGuid()}&amount={amount}&orderId={orderId}&orderInfo=payment&returnUrl=returnUrl&notifyUrl=notifyUrl&extraData=";
            return HmacSHA256(rawHash, _secretKey);
        }

        private string HmacSHA256(string data, string key)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

    }
}

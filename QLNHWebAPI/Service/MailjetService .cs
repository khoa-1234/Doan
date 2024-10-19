using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QLNHWebAPI.Models;

namespace QLNHWebAPI.Service
{
    public class MailjetService
    {
         private readonly MailjetClient _client;

            public MailjetService(string apiKey, string apiSecret)
            {
                _client = new MailjetClient(apiKey, apiSecret);
            }

            public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
            {
                var request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, "dangkhoavh9a3.2015@gmail.com")
                .Property(Send.FromName, "Phan Khoa")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, body)
                .Property(Send.To, toEmail);

                var response = await _client.PostAsync(request);

                return response.IsSuccessStatusCode;
            }
        }
    
}

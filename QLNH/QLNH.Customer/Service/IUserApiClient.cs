using QLNH.Data.Models;
using QLNH.Data.ViewModels;
namespace QLNH.Customer.Service
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginReQuest loginRequest);
        HttpClient CreateClientWithToken(string token);
        Task<ResponeMessage> GetDataWithoutToken(string url);
    }
}

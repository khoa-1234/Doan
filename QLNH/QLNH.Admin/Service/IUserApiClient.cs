using QLNH.Data.ViewModels;
namespace QLNH.Admin.Service
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginReQuest loginRequest);
        HttpClient CreateClientWithToken(string token);
    }
}

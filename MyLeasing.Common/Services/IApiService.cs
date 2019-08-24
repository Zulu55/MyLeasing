using System.Threading.Tasks;
using MyLeasing.Common.Models;

namespace MyLeasing.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetOwnerByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, string email);

        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);
    }
}
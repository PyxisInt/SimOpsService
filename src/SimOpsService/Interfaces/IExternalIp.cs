using Refit;
using System.Threading.Tasks;

namespace SimOpsService.Interfaces
{
    public interface IExternalIp
    {
        [Get("")]
        Task<string> GetMyExternalIp();
    }
}
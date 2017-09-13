using System.Threading.Tasks;

namespace WikiImagesProcessor.Abstractions.Services
{
    public interface IJsonHttpService
    {
        Task<T> GetJson<T>(string uri);
    }
}
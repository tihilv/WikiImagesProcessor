using System.Threading.Tasks;
using WikiImagesProcessor.Abstractions.Model;

namespace WikiImagesProcessor.Abstractions.Services
{
    public interface ICoordinateService
    {
        Task<Coordinates> GetCoordinatesByName(string locationName);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiImagesProcessor.Abstractions.Model;

namespace WikiImagesProcessor.Abstractions.Services
{
    public interface IWikiService
    {
        Task<IEnumerable<ArticleInfo>> GetGeoArticles(string subdomain, Coordinates coordinates, int limit);
        Task<IEnumerable<ImageInfo>> GetArticleImages(string subdomain, IEnumerable<ArticleInfo> articleInfos);
    }
}
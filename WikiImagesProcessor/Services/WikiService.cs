using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WikiImagesProcessor.Abstractions.Model;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{
    class WikiService : IWikiService
    {
        private readonly IJsonHttpService _jsonHttpService;
        private readonly ILogService _logService;

        public WikiService(IJsonHttpService jsonHttpService, ILogService logService)
        {
            _jsonHttpService = jsonHttpService;
            _logService = logService;
        }

        public async Task<IEnumerable<ArticleInfo>> GetGeoArticles(string subdomain, Coordinates coordinates, int limit)
        {
            _logService.Info($"Requesting {limit} articles in {subdomain} language for coordinates '{coordinates.ToGpsCoords()}'.");

            long radius = 10000;

            var response = await _jsonHttpService.GetJson<ArticlesInfo>($"https://{subdomain}.wikipedia.org/w/api.php?action=query&list=geosearch&gsradius={radius}&gscoord={coordinates.ToGpsCoords()}&gslimit={limit}&format=json");

            CheckError(response.Error);

            var result = response.Query.Geosearch.Select(g => new ArticleInfo(g.PageId, g.Title));

            if (response.Query.Geosearch.Length < limit)
                _logService.Warning($"Insufficient number of articles obtained: {response.Query.Geosearch.Length} but requested {limit}.");
            else
                _logService.Info($"Aritcles obtained: {response.Query.Geosearch.Length}.");

            return result;
        }

        public async Task<IEnumerable<ImageInfo>> GetArticleImages(string subdomain, IEnumerable<ArticleInfo> articleInfos)
        {
            var enumerableArticles = articleInfos as ArticleInfo[] ?? articleInfos.ToArray();
            var articleNames = string.Join(",", enumerableArticles.Select(a => a.Title));
            var articleIds = string.Join("|", enumerableArticles.Select(a => a.PageId));

            _logService.Info($"Requesting images for {subdomain} language for articles '{articleNames}'.");

            string baseRequest = $"https://{subdomain}.wikipedia.org/w/api.php?action=query&prop=images&pageids={articleIds}&format=json";
            string imContinue = null;

            List<ImageInfo> result = new List<ImageInfo>();

            do
            {
                var request = baseRequest;
                if (imContinue != null)
                    request = $"{request}&imcontinue={imContinue}";
                var response = await _jsonHttpService.GetJson<ImagesInfo>(request);

                CheckError(response.Error);

                foreach (ImagesInfo.Page page in response.Query.Pages.Values)
                    if (page.Images != null)
                        foreach (ImagesInfo.Image image in page.Images)
                            result.Add(new ImageInfo(page.PageId, page.Title, image.Ns, image.Title));

                imContinue = response.Continue?.ImContinue;

            } while (imContinue != null);

            _logService.Info($"Images obtained for articles '{articleNames}': {result.Count}.");

            return result;
        }

        private static void CheckError(ErrorInfo error)
        {
            if (error != null)
                throw new Exception($"{error.Code} {error.Info}");
        }

        internal class ErrorInfo
        {
            [JsonProperty("info")]
            public string Info { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }

        private class ArticlesInfo
        {
            [JsonProperty("error")]
            public ErrorInfo Error { get; set; }

            [JsonProperty("query")]
            public QueryInfo Query { get; set; }

            internal class QueryInfo
            {
                [JsonProperty("geosearch")]
                public GeosearchInfo[] Geosearch { get; set; }
            }

            internal class GeosearchInfo
            {
                [JsonProperty("pageid")]
                public long PageId { get; set; }

                [JsonProperty("title")]
                public string Title { get; set; }
            }
        }

        private class ImagesInfo
        {
            [JsonProperty("error")]
            public ErrorInfo Error { get; set; }

            [JsonProperty("query")]
            public QueryInfo Query { get; set; }

            [JsonProperty("continue")]
            public ContinueInfo Continue { get; set; }

            internal class ContinueInfo
            {
                [JsonProperty("continue")]
                public string OtherContinue { get; set; }

                [JsonProperty("imcontinue")]
                public string ImContinue { get; set; }
            }

            internal class QueryInfo
            {
                [JsonProperty("pages")]
                public Dictionary<string, Page> Pages { get; set; }
            }
            
            public class Page
            {
                [JsonProperty("ns")]
                public long Ns { get; set; }

                [JsonProperty("images")]
                public Image[] Images { get; set; }

                [JsonProperty("pageid")]
                public long PageId { get; set; }

                [JsonProperty("title")]
                public string Title { get; set; }
            }

            public class Image
            {
                [JsonProperty("ns")]
                public long Ns { get; set; }

                [JsonProperty("title")]
                public string Title { get; set; }
            }
        }
    }
}

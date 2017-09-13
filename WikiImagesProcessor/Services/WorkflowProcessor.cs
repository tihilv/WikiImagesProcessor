using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiImagesProcessor.Abstractions.Model;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{

    class WorkflowProcessor : IWorkflowProcessor
    {
        private readonly ILogService _logService;
        private readonly ICoordinateService _coordinateService;
        private readonly IWikiService _wikiService;
        private readonly IDistanceService _distanceService;

        public WorkflowProcessor(ICoordinateService coordinateService, IWikiService wikiService, IDistanceService distanceService, ILogService logService)
        {
            _coordinateService = coordinateService;
            _wikiService = wikiService;
            _distanceService = distanceService;
            _logService = logService;
        }

        public async Task<List<Tuple<ImageInfo, ImageInfo>>> Process(ProcessOptions processOptions)
        {
            var coordinates = await _coordinateService.GetCoordinatesByName(processOptions.LocationName);
            var articles = await _wikiService.GetGeoArticles(processOptions.Subdomain, coordinates, processOptions.ArticlesLimit);
            var images = await GetArticleImagesParallel(processOptions.Subdomain, articles);
            
            return FindMostSimilar(images, processOptions.IgnoreEqualTitles);
        }

        public async Task<IEnumerable<ImageInfo>> GetArticleImagesParallel(string subdomain, IEnumerable<ArticleInfo> articleInfos)
        {
            var enumerable = articleInfos as ArticleInfo[] ?? articleInfos.ToArray();
            int pagesPerRequest = 7;
            List<Task<IEnumerable<ImageInfo>>> tasks = new List<Task<IEnumerable<ImageInfo>>>();

            for (int i = 0; i < enumerable.Length; i += pagesPerRequest)
                tasks.Add(_wikiService.GetArticleImages(subdomain, enumerable.Skip(i).Take(pagesPerRequest)));

            var imagesResult = await Task.WhenAll(tasks);
            return imagesResult.SelectMany(r => r);
        }

        private List<Tuple<ImageInfo, ImageInfo>> FindMostSimilar(IEnumerable<ImageInfo> images, bool ignoreEqualTitles)
        {
            var imageArray = images.ToArray();

            _logService.Info($"Total images obtained: {imageArray.Length}");

            List<Tuple<ImageInfo, ImageInfo>> mostSimilar = null;
            int minDistance = Int32.MaxValue;

            for (int i = 0; i < imageArray.Length; i++)
            for (int j = i + 1; j < imageArray.Length; j++)
            {
                var s1 = imageArray[i].Title.ToLower();
                var s2 = imageArray[j].Title.ToLower();
                if (!(ignoreEqualTitles && s1.Equals(s2, StringComparison.OrdinalIgnoreCase)))
                {
                    var dist = _distanceService.GetDistance(s1, s2);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        mostSimilar = new List<Tuple<ImageInfo, ImageInfo>>();
                    }
                    if (dist == minDistance)
                        mostSimilar?.Add(new Tuple<ImageInfo, ImageInfo>(imageArray[i], imageArray[j]));
                }
            }

            _logService.Info($"Most simiar images: {mostSimilar?.Count??0}");

            return mostSimilar;
        }
    }
}

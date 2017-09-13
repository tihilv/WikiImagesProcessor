using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{
    class JsonHttpService : IJsonHttpService
    {
        private readonly ILogService _logService;

        public JsonHttpService(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<T> GetJson<T>(string uri)
        {
            _logService.Trace($"Requesting uri: '{uri}'");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                var jsonString = await response.Content.ReadAsStringAsync();

                _logService.Trace($"Response: '{jsonString}'");

                return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }
    }
}

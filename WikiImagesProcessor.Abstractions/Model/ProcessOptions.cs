namespace WikiImagesProcessor.Abstractions.Model
{
    public class ProcessOptions
    {
        public readonly string LocationName;

        public readonly int ArticlesLimit;

        public readonly string Subdomain;

        public readonly bool IgnoreEqualTitles;

        public ProcessOptions(string locationName, int articlesLimit, string subdomain, bool ignoreEqualTitles)
        {
            LocationName = locationName;
            ArticlesLimit = articlesLimit;
            Subdomain = subdomain;
            IgnoreEqualTitles = ignoreEqualTitles;
        }
    }
}

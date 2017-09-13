namespace WikiImagesProcessor.Abstractions.Model
{
    public struct ImageInfo
    {
        public readonly long ArticlePageId;
        public readonly string ArticleTitle;

        public readonly long Id;
        public readonly string Title;


        public ImageInfo(long articlePageId, string articleTitle, long id, string title)
        {
            ArticlePageId = articlePageId;
            ArticleTitle = articleTitle;
            Id = id;
            Title = title;
        }

        public override string ToString()
        {
            return $"'{Title}' from '{ArticleTitle}'";
        }
    }
}
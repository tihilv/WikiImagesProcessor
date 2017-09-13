namespace WikiImagesProcessor.Abstractions.Model
{
    public struct ArticleInfo
    {
        public readonly long PageId;
        public readonly string Title;

        public ArticleInfo(long pageId, string title)
        {
            PageId = pageId;
            Title = title;
        }
    }
}
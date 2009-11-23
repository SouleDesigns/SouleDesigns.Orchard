namespace Orchard.Models {
    public abstract class ContentPart : IContent {
        public ContentItem ContentItem { get; set; }
    }

    public class ContentPart<TRecord> : ContentPart {
        public TRecord Record { get; set; }
    }

}
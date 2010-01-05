using System;
using System.Web.Mvc;
using Orchard.UI.Zones;

namespace Orchard.ContentManagement.ViewModels {
    public class ItemDisplayModel : IZoneContainer {
        private ContentItem _item;

        protected ItemDisplayModel() {
            Zones = new ZoneCollection();
        }

        protected ItemDisplayModel(ItemDisplayModel displayModel) {
            TemplateName = displayModel.TemplateName;
            Prefix = displayModel.Prefix;
            Item = displayModel.Item;
            Zones = displayModel.Zones;
        }

        public ItemDisplayModel(ContentItem item) {
            Zones = new ZoneCollection();
            Item = item;
        }

        public ContentItem Item {
            get { return _item; }
            set { SetItem(value); }
        }

        protected virtual void SetItem(ContentItem value) {
            _item = value;
        }

        public Func<HtmlHelper, ItemDisplayModel, HtmlHelper> Adaptor { get; set; }
        public string TemplateName { get; set; }
        public string Prefix { get; set; }
        public ZoneCollection Zones { get; set; }
    }

    public class ItemDisplayModel<TPart> : ItemDisplayModel where TPart : IContent {
        private TPart _item;

        public ItemDisplayModel() {

        }

        public ItemDisplayModel(ItemDisplayModel displayModel)
            : base(displayModel) {
        }

        public ItemDisplayModel(TPart part)
            : base(part.ContentItem) {
        }

        public new TPart Item {
            get { return _item; }
            set { SetItem(value.ContentItem); }
        }

        protected override void SetItem(ContentItem value) {
            _item = value.As<TPart>();
            base.SetItem(value);
        }
    }
}
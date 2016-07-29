using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using JsInfiniteAjaxScroll.Models;
using JsInfiniteAjaxScroll.Settings;
using Orchard.UI.Notify;
using Orchard.Localization;

namespace JsInfiniteAjaxScroll.Drivers
{
    public class InfiniteAjaxScrollingPartDriver : ContentPartDriver<InfiniteAjaxScrollingPart>
    {
        private const string TemplateName = "Parts/JsInfiniteAjaxScroll";
        private readonly INotifier _notifier;
        public Localizer T { get; set; }

        public InfiniteAjaxScrollingPartDriver(INotifier notifier)
        {
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(InfiniteAjaxScrollingPart part, string displayType, dynamic shapeHelper)
        {            
            return ContentShape("Parts_JsInfiniteAjaxScroll",
                                () => shapeHelper.Parts_JsInfiniteAjaxScroll(ContentItem: part.ContentItem));
        }

        protected override DriverResult Editor(InfiniteAjaxScrollingPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_JsInfiniteAjaxScroll",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(InfiniteAjaxScrollingPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                var settings = part.Settings.GetModel<InfiniteAjaxScrollingTypePartSettings>();

                part.Record.UseHistory = part.UseHistory != settings.UseHistory ? part.UseHistory : settings.UseHistory;
                part.Record.Container = part.Container != settings.Container ? part.Container : settings.Container;
                part.Record.Item = part.Item != settings.Item ? part.Item : settings.Item;
                part.Record.Loader = part.Loader != settings.Loader ? part.Loader : settings.Loader;
                part.Record.Pagination = part.Pagination != settings.Pagination ? part.Pagination : settings.Pagination;
                part.Record.NextAnchor = part.NextAnchor != settings.NextAnchor ? part.NextAnchor : settings.NextAnchor;
                part.Record.OnLoadItems = part.OnLoadItems != settings.OnLoadItems ? part.OnLoadItems : settings.OnLoadItems;
                part.Record.BeforePageChange = part.BeforePageChange != settings.BeforePageChange ? part.BeforePageChange : settings.BeforePageChange;
                part.Record.OnPageChange = part.OnPageChange != settings.OnPageChange ? part.OnPageChange : settings.OnPageChange;
                part.Record.OnRenderComplete = part.OnRenderComplete != settings.OnRenderComplete ? part.OnRenderComplete : settings.OnRenderComplete;
                _notifier.Information(T("Infinite Ajax Scrolling edited successfully"));
            }
            else
            {
                _notifier.Error(T("Error during Infinite Ajax Scrolling update!"));
            }
            return Editor(part, shapeHelper);
        }
    }
}
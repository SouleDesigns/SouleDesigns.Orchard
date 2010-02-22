﻿using System.Linq;
using System.Web.Mvc;
using Orchard.Blogs.Extensions;
using Orchard.Blogs.Models;
using Orchard.Blogs.ViewModels;
using Orchard.Mvc.Results;
using Orchard.Services;
using Orchard.Core.Feeds;

namespace Orchard.Blogs.Services {
    public class BlogHomePageProvider : IHomePageProvider {
        private readonly IBlogService _blogService;
        private readonly IFeedManager _feedManager;

        public BlogHomePageProvider(IOrchardServices services, IBlogService blogService, IFeedManager feedManager) {
            Services = services;
            _feedManager = feedManager;
            _blogService = blogService;
        }

        public IOrchardServices Services { get; private set; }

        #region Implementation of IHomePageProvider

        public string GetProviderName() {
            return "BlogHomePageProvider";
        }

        public ActionResult GetHomePage(int itemId) {
            Blog blog = _blogService.Get().Where(x => x.Id == itemId).FirstOrDefault();

            if (blog == null)
                return new NotFoundResult();

            var model = new BlogViewModel {
                Blog = Services.ContentManager.BuildDisplayModel(blog, "Detail")
            };

            _feedManager.Register(blog);

            return new ViewResult {
                ViewName = "~/Modules/Orchard.Blogs/Views/Blog/Item.ascx",
                ViewData = new ViewDataDictionary<BlogViewModel>(model)
            };
        }

        #endregion
    }
}
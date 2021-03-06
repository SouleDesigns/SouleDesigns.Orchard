﻿using System.Collections.Generic;
using System.Linq;
using Contrib.ImportExport.Helpers;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard.Autoroute.Models;
using Orchard.Blogs.Models;
using Orchard.Blogs.Services;
using Orchard.Comments.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;

namespace Contrib.ImportExport.Services.Strategies {
    public class BlogPostImportStrategy : IBlogPostImportStrategy {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IMultipleImportStrategy> _importStratagies;
        private readonly IUserServices _userServices;
        private readonly IDataCleaner _dataCleaner;
        private readonly IBlogPostService _blogPostService;

        public BlogPostImportStrategy(IContentManager contentManager, 
            IEnumerable<IMultipleImportStrategy> importStratagies, 
            IUserServices userServices,
            IDataCleaner dataCleaner,
            IBlogPostService blogPostService) {
            _contentManager = contentManager;
            _importStratagies = importStratagies;
            _userServices = userServices;
            _dataCleaner = dataCleaner;
            _blogPostService = blogPostService;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is Post;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            Post blogPostToImport = (Post)objectToImport;

            var blogPostPart = _blogPostService.Get(parentContent.As<BlogPart>()).FirstOrDefault(rr => rr.As<IAliasAspect>().Path == blogPostToImport.PostUrl);

            if (importSettings.Override && blogPostPart != null)
                blogPostPart = _contentManager.Get(blogPostPart.ContentItem.Id, VersionOptions.DraftRequired).As<BlogPostPart>();

            ContentItem contentItem = null;

            if (blogPostPart != null) {
                contentItem = blogPostPart.ContentItem;

                if (!importSettings.Override) {
                    if (blogPostToImport.DateModified.IsEarlierThan(contentItem.As<ICommonPart>().ModifiedUtc)
                        || blogPostToImport.DateModified.Equals(contentItem.As<ICommonPart>().ModifiedUtc)) {
                        return contentItem;
                    }
                }
            }
            else {
                contentItem = _contentManager.Create("BlogPost", VersionOptions.Draft);

                contentItem.As<ICommonPart>().Container = parentContent;

                if (!string.IsNullOrEmpty(blogPostToImport.PostUrl))
                    contentItem.As<AutoroutePart>().DisplayAlias = blogPostToImport.PostUrl.TrimStart('/');
            }

            contentItem.As<TitlePart>().Title = blogPostToImport.Title;
            contentItem.As<BodyPart>().Text = _dataCleaner.Clean(blogPostToImport.Content.Value, importSettings);

            var user = blogPostToImport.Authors.AuthorReferenceList.FirstOrDefault();
            if (user != null) {
                contentItem.As<ICommonPart>().Owner = _userServices.GetUser(user.ID, true);
            }

            contentItem.As<CommentsPart>().CommentsActive = blogPostToImport.Comments.Enabled;

            if (blogPostToImport.HasExcerpt)
                ((dynamic) contentItem).ExcerptPart.Excerpt.Value = blogPostToImport.Excerpt.Value;

            ImportAdditionalContentItems(importSettings, blogPostToImport.Categories, contentItem);
            ImportAdditionalContentItems(importSettings, blogPostToImport.Tags, contentItem);
            ImportAdditionalContentItems(importSettings, blogPostToImport.Comments, contentItem);

            if (blogPostToImport.DateCreated.IsNotEmpty())
            {
                contentItem.As<ICommonPart>().ModifiedUtc = blogPostToImport.DateCreated;
                contentItem.As<ICommonPart>().VersionModifiedUtc = blogPostToImport.DateCreated;
            }

            if (blogPostToImport.DateCreated.IsNotEmpty()) {
                contentItem.As<ICommonPart>().CreatedUtc = blogPostToImport.DateCreated;
                contentItem.As<ICommonPart>().VersionCreatedUtc = blogPostToImport.DateCreated;
            }

            if (blogPostToImport.Approved)
            {
                if (blogPostToImport.DateCreated.IsNotEmpty())
                {
                    contentItem.As<ICommonPart>().PublishedUtc = blogPostToImport.DateCreated;
                    contentItem.As<ICommonPart>().VersionPublishedUtc = blogPostToImport.DateCreated;
                }
                _contentManager.Publish(contentItem);
            }                                                            

            return contentItem;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
            foreach (var importStrategy in _importStratagies) {
                if (importStrategy.IsType(objectToImport)) {
                    importStrategy.Import(importSettings, objectToImport, parentContent);
                }
            }
        }
    }
}

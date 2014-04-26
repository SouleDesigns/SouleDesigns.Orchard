using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Contrib.ImportExport.InternalSchema;
using Contrib.ImportExport.InternalSchema.Author;
using Contrib.ImportExport.InternalSchema.Category;
using Contrib.ImportExport.InternalSchema.Common;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.InternalSchema.Post.Additional;
using Contrib.ImportExport.InternalSchema.Tag;
using Google.GData.Client;
using Google.GData.Blogger;
using Orchard;
using Orchard.Localization;
using Orchard.Services;
using Orchard.UI.Notify;

namespace Contrib.ImportExport.Providers.Blogger {
    public class BloggerBlogAssembler : IBlogAssembler {
        private readonly IOrchardServices _orchardServices;
        private readonly IClock _clock;

        public BloggerBlogAssembler(IOrchardServices orchardServices, IClock clock)
        {
            _orchardServices = orchardServices;
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string Name {
            get { return "Blogger"; }
        }

        public bool IsFeed {
            get { return false; }
        }

        public Blog Assemble(Stream stream) {
            
            var feed = DeserializeBloggerStream(stream);
            
            Blog blog = CreateTopLevelBlog(feed);
            GetTags(blog, feed);
            GetPosts(blog, feed);

            return blog;
        }

        private Blog CreateTopLevelBlog(BloggerFeed feed)
        {
            Blog blog = new Blog(_clock);

            BloggerEntry bloggerEntry = null;
            
            bloggerEntry = feed.Entries.Where(entry => entry.SelfUri.ToString().EndsWith("BLOG_DATE_FORMAT")).FirstOrDefault() as BloggerEntry;
            blog.DateCreated = bloggerEntry.Published;

            bloggerEntry = feed.Entries.Where(entry => entry.SelfUri.ToString().EndsWith("BLOG_NAME")).FirstOrDefault() as BloggerEntry;
            blog.Title = new Title();
            blog.Title.Value = bloggerEntry.Content.Content;

            bloggerEntry = feed.Entries.Where(entry => entry.SelfUri.ToString().EndsWith("BLOG_DESCRIPTION")).FirstOrDefault() as BloggerEntry;
            blog.SubTitle = new Title();
            blog.SubTitle.Value = bloggerEntry.Content.Content;

            blog.RootURL = "/blog"; // string.Empty;
            blog.Authors = new Authors();
            blog.Categories = new Categories();
            blog.Tags = new Tags();
            blog.Posts = new Posts();

            return blog;
        }

        private void GetTags(Blog blog, BloggerFeed feed)
        {

            // Get tags from feed
            var tagStrings = feed.Entries
                .SelectMany(ent => ent.Categories)
                .Where(cat => cat.Scheme == "http://www.blogger.com/atom/ns#")
                .Select(cat => cat.Term)
                .Distinct()
                .ToList();                        

            // Populate blog tags
            foreach (string tagString in tagStrings)
            {
                // Add tag
                blog.Tags.TagList.Add(new Tag() { 
                    DateCreated = blog.DateCreated, 
                    DateModified = feed.Updated,
                    Approved = true,
                    ID = GetNewId(), 
                    Title = tagString,
                    Slug = null
                });                                
            }            
        }

        private void GetPosts(Blog blog, BloggerFeed feed)
        {
            var feedBaseUri = feed.Feed.Replace("archive", string.Empty);
            var bloggerEntries = feed.Entries
                    .Where(entry => entry.SelfUri.ToString().Replace(feedBaseUri, string.Empty).StartsWith("posts"))
                    .OrderByDescending(entry => entry.Published)
                    .Cast<BloggerEntry>()
                    .ToList();

            // Iterate across the posts            
            foreach (var bloggerEntry in bloggerEntries)
            {
                Post post = new Post();

                post.ID = bloggerEntry.SelfUri.ToString().Substring(bloggerEntry.SelfUri.ToString().LastIndexOf('/') + 1);
                post.Title = bloggerEntry.Title.Text;


                // Get tag by joining to already defined blog tag list
                // from this entry's tags list
                var tags = bloggerEntry
                    .Categories
                    .Where(cat => cat.Scheme == "http://www.blogger.com/atom/ns#")
                    .Join(blog.Tags.TagList, cat => cat.Term, tag => tag.Title, (cat, tag) => tag)
                    .ToList();

                // Add tag references
                foreach (var tag in tags)
                {
                    // Assure list exists
                    if (post.Tags == null)
                        post.Tags = new TagReferences();
                    
                    // Add reference to list
                    post.Tags.TagReferenceList.Add(new TagReference()
                    {
                        ID = tag.ID,
                        Title = tag.Title
                    });
                }

                // Iterate entry authors
                foreach (var entryAuthor in bloggerEntry.Authors)
                {
                    // Locate author
                    var author = blog.Authors.AuthorList.Where(e => e.Email == entryAuthor.Email).FirstOrDefault();

                    // If not found, add it yo!
                    if (author == null)
                    {
                        // Build author
                        author = new Author()
                        {
                            Approved = true,
                            DateCreated = blog.DateCreated,
                            DateModified = feed.Updated,
                            ID = GetNewId(),
                            Email = entryAuthor.Email,
                            Title = entryAuthor.Name
                        };

                        // Assure list exits
                        if (blog.Authors == null)
                            blog.Authors = new Authors();

                        // Add to list
                        blog.Authors.AuthorList.Add(author);
                    }

                    // Assure list exists
                    if (post.Authors == null)
                        post.Authors = new AuthorReferences();

                    // Add post author reference
                    post.Authors.AuthorReferenceList.Add(new AuthorReference() { ID = author.ID });                    
                }

                // Add comments
                var bloggerCommentEntries = feed.Entries.Where(entry => 
                    entry
                        .SelfUri.ToString()
                        .Contains(string.Format("{0}/comments/", post.ID)))
                    .Cast<BloggerEntry>()                        
                    .ToList();
                
                foreach (var bloggerCommentEntry in bloggerCommentEntries)
                {
                    Comment comment = new Comment();

                    // Check for name - owen soule - if not add as anonymous
                    // All the comments from the blogger atom use the noreply@blogger.com
                    // email address which makes this logic think they are all the same user

                    comment.ID = GetNewId();
                    comment.Approved = true;
                    comment.Content = new Content { Type = Content.TypeHTML, Value = bloggerCommentEntry.Content.Content };
                    comment.DateCreated = bloggerCommentEntry.Updated;
                    comment.DateModified = bloggerCommentEntry.Updated;
                    comment.Title = bloggerCommentEntry.Title.Text;

                    if (comment.UserName == "Owen Soule")
                    {
                        comment.UserEmail = bloggerCommentEntry.Authors.First().Email;
                        comment.UserName = bloggerCommentEntry.Authors.First().Name;
                        comment.UserURL = bloggerCommentEntry.Authors.First().Uri.ToString();
                    }

                    post.Comments.CommentList.Add(comment);
                }                                              

                // Get approved based on app control presence
                bool approved = true;
                if(bloggerEntry.AppControl != null && bloggerEntry.AppControl.Draft != null && bloggerEntry.AppControl.Draft.BooleanValue) 
                    approved = false;

                post.Approved = approved;                               
                post.Content = new Content { Type = Content.TypeHTML, Value = bloggerEntry.Content.Content};
                post.DateCreated = bloggerEntry.Published;
                post.DateModified = bloggerEntry.Updated;
                post.HasExcerpt = false;// N/A                
                post.PostName = new Title { Type = Content.TypeText, Value = bloggerEntry.Title.Text };
                post.PostUrl = bloggerEntry.AlternateUri == null ? bloggerEntry.SelfUri.ToString() : bloggerEntry.AlternateUri.ToString();
                
                if (post.PostUrl.Contains("http://blog.souledesigns.com"))
                {
                    post.PostUrl = post.PostUrl.Replace("http://blog.souledesigns.com", "/blog").Replace(".html", "");
                }
                else
                {
                    post.PostUrl = null;
                }

                post.Title = bloggerEntry.Title.Text;
                post.Views = 0;         // N/A

                blog.Posts.PostList.Add(post);

            }
         
        }

        private string GetAuthorReference(Blog blog, string author)
        {

            string id = author; // Constants.Slug(author);

            IEnumerable<Author> authors =
                from a in blog.Authors.AuthorList
                where a.ID == id
                select a;

            if (!authors.Any())
            {
                // Author not found - let's create them!
                Author newAuthor = new Author();
                newAuthor.ID = id;
                newAuthor.Email = id + "@" + (new Uri(blog.RootURL)).Host;
                newAuthor.Title = author;
                blog.Authors.AuthorList.Add(newAuthor);
            }

            return id;
        }

        private string GetNewId() 
        {
            return Regex.Replace(Guid.NewGuid().ToString(), @"\{\-", "");
        }
        
        private BloggerFeed DeserializeBloggerStream(Stream stream)
        {
            try
            {                
                var feed = new BloggerFeed(null, null);                
                feed.Parse(stream, AlternativeFormat.Atom);
                
                return feed;                
            }
            catch (Exception ex)
            {
                _orchardServices.Notifier.Error(T("Error deserializing your blog Error:{0} - Please verify that this is an XML file stream", ex.Message));
                throw;
            }
        }
    }
}
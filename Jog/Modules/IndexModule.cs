using System;
using System.Linq;
using Jog.Features;
using Jog.Models;
using Nancy;

namespace Jog.Modules
{
    public class IndexModule : BaseModule
    {
        public IndexModule()
        {
            Get["/"] = p => IndexAction(new RecentBlogPostsBindingModel { Page = 1, Take = 10 });
            Get["/{Id}"] = p => PageAction(p.Id);
            Get["/page/{page:int}"] = p =>
                IndexAction(new RecentBlogPostsBindingModel { Page = p.page, Take = 10 });
            Get[@"/(?<year>\d{4})/(?<month>0[1-9]|1[0-2])/(?<Id>[a-zA-Z0-9_-]+)"] = p =>
                PostAction(p.Id);
            Get["/tag/{Tag}"] = p => IndexAction(new RecentBlogPostsBindingModel {Page = 1, Take = 10}, p.Tag);
            Get["/tag/{Tag}/page/{page:int}"] = p => IndexAction(new RecentBlogPostsBindingModel { Page = p.page, Take = 10 }, p.Tag);
            Get["/make.html"] = p => Make();
        }

        public dynamic IndexAction(RecentBlogPostsBindingModel input,string tag = "")
        {
            var model = UniversalHelper.Project(input, BlogPosts, tag);
            
            if (!model.Posts.Any())
            {
                return input.Page > 1 ? HttpStatusCode.NotFound : Response.AsText("Just1n尚未发表任何文章!", "text/html; charset=utf-8");
            }
            ViewBag.Tag = tag;
            return View["index",model];
        }

        private dynamic PageAction(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var model =
                BlogPosts.FirstOrDefault(p => p.Status == PostStatus.Publish && p.Type == PostType.Page && p.Id.ToLower() == id.ToLower());
            return View["page", model];
        }

        public dynamic PostAction(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var model =
                BlogPosts.FirstOrDefault(p => p.Status == PostStatus.Publish && p.Type == PostType.Post && p.Id.ToLower() == id.ToLower());
            return View["post", model];
        }

        public dynamic Make()
        {
            ReFetchPosts();
            return Response.AsRedirect("/");
        }
    }
}
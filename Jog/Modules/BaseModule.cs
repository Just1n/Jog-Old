using System.Collections.Generic;
using System.Linq;
using Jog.Features;
using Jog.Models;
using Mono.Posix;
using Nancy;

namespace Jog.Modules
{
    public class BaseModule : NancyModule
    {
        static BaseModule()
        {
            BlogPosts = UniversalHelper.FetchPosts();
        }

        public static IEnumerable<BlogPost> BlogPosts { get; set; }

        public BaseModule()
        {
            Before += SetViewBagWithSettings;
        }


        public void ReFetchPosts()
        {
            BlogPosts = UniversalHelper.FetchPosts(true);
        }
        private Response SetViewBagWithSettings(NancyContext arg)
        {
            ViewBag.Settings = AppConfiguration.Current;
            ViewBag.Pages = BlogPosts.Where(p => p.Status == PostStatus.Publish && p.Type == PostType.Page).ToList();
            return null;
        }
    }
}

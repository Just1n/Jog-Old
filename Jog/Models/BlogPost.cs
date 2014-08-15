using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using MarkdownSharp;

namespace Jog.Models
{
    public class BlogPost
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public PostStatus Status { get; set; }
        public PostType Type { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }

        public string GetLink()
        {
            return string.Format("/{0}/{1}",Date.ToString("yyyy/MM",CultureInfo.InvariantCulture),Id);
        }

        public string GetTransContent()
        {
            return new Markdown().Transform(Content);
        }

        public string GetRelTime()
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - Date.ToUniversalTime().Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;
            #region 计算相对时间
            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * minute)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * minute)
            {
                return "a minute ago";
            }
            if (delta < 45 * minute)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * minute)
            {
                return "an hour ago";
            }
            if (delta < 24 * hour)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * hour)
            {
                return "yesterday";
            }
            if (delta < 30 * day)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * month)
            {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";

            #endregion
        }
    }

    public class RecentBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public int CurrentPage { get; set; }

        public int Pages { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return CurrentPage > 1;
            }
        }
    }

    public class RecentBlogPostsBindingModel
    {
        public RecentBlogPostsBindingModel()
        {
            Page = 1;
            Take = 10;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }
}

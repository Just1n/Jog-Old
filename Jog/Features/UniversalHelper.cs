using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jog.Models;
using Nancy;
using NGit;
using NGit.Api;
using NGit.Storage.File;

namespace Jog.Features
{
    public static class UniversalHelper
    {
        public static IEnumerable<BlogPost> FetchPosts(bool reClone = false)
        {
            var workdirPath = Environment.CurrentDirectory + @"\Posts";
            if (reClone & Directory.Exists(workdirPath))
            {
                if(!Directory.Exists(workdirPath + @"\.git"))
                {
                    ForceDeleteDirectory(workdirPath);
                }
                else
                {
                    var git = new Git(new FileRepository(workdirPath + @"\.git"));
                    git.Checkout().SetAllPaths(true).Call();
                    git.Pull().Call();   
                }
            }
            if (!Directory.Exists(workdirPath))
            {
                Git.CloneRepository().SetDirectory(workdirPath).SetURI(AppConfiguration.Current.PostsUrl).Call();
            }
            return Directory.EnumerateFiles(workdirPath, "*.md").Select(File.ReadAllText).ToList().ToBlogPostList();
        }

        private static void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }

        public static RecentBlogPostsViewModel Project(RecentBlogPostsBindingModel input,IEnumerable<BlogPost> blogPosts,string tag = "")
        {
            var allPosts = tag == ""
                ? blogPosts.Where(p => p.Status == PostStatus.Publish && p.Type == PostType.Post).ToList()
                : blogPosts.Where(
                    p => p.Status == PostStatus.Publish && p.Type == PostType.Post && p.Tags.Contains(tag)).ToList();
            var tabkePosts =
                    allPosts.OrderByDescending(p => p.Date).TakePage(input.Page, pageSize: input.Take + 1).ToList();
            var count = allPosts.Count;
   
            var pagedPosts = tabkePosts.Take(input.Take).ToList();
            var hasNextPage = tabkePosts.Count > input.Take;

            return new RecentBlogPostsViewModel
            {
                Posts = pagedPosts,
                CurrentPage = input.Page,
                HasNextPage = hasNextPage,
                Pages = count%input.Take == 0 ? count/input.Take : count/input.Take + 1
            };
        }
    }

    public static class MyExtensions
    {
        public static IEnumerable<BlogPost> ToBlogPostList(this IEnumerable<string> mds)
        {
            var result = new List<BlogPost>();
            if (mds == null)
            {
                return result;
            }

            mds.ToList().ForEach(md =>
            {
                if (md != null)
                {
                    var header = md.Substring(4, md.IndexOf("-->", StringComparison.Ordinal) - 4).Trim();
                    var content = md.Substring(md.IndexOf("-->", StringComparison.Ordinal) + 4).Trim();
                    var blogPost = new BlogPost();

                    header.Split('\n').ToList().ForEach(p =>
                    {
                        var line = p.Split('|');
                        var value = line[1].Trim();
                        switch (line[0].Trim().ToLower())
                        {
                            case "title":
                                blogPost.Title = value;
                                break;
                            case "id":
                                blogPost.Id = value;
                                break;
                            case "date":
                                blogPost.Date = Convert.ToDateTime(value);
                                break;
                            case "status":
                                blogPost.Status = value.ToLower() == "publish" ? PostStatus.Publish : PostStatus.Draft;
                                break;
                            case "type":
                                blogPost.Type = value.ToLower() == "post" ? PostType.Post : PostType.Page;
                                break;
                            case "tags":
                                blogPost.Tags = value.Split(',').ToList();
                                break;
                            case "excerpt":
                                blogPost.Excerpt = value;
                                break;
                        }

                    });
                    blogPost.Content = content;
                    result.Add(blogPost);
                }
            });

            return result;
        }

        public static IEnumerable<T> TakePage<T>(this IEnumerable<T> queryable, int page = 1, int pageSize = 10)
        {
            return queryable.Skip((page - 1) * pageSize - 1).Take(pageSize);
        }
    }
}

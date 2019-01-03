using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TRKS.github.ImageDownloader
{
    public class ImageDownloader
    {
        private static volatile int count = 1;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DownloadPictures(List<string> urls, string path)
        {
            ServicePointManager.DefaultConnectionLimit = 10000;

            Parallel.ForEach(
                urls,
                new ParallelOptions {MaxDegreeOfParallelism = 20}, url =>
                {
                    var strs = url.Split('/');
                    var current = count;
                    count++;
                    Console.WriteLine($"第{current}/{urls.Count}个文件的下载开始.");
                    WebHelper.DowloadFile(url, path, strs.Last());
                    Console.WriteLine($"第{current}/{urls.Count}个文件的下载结束.");
                });
        }
        static void Main()
        {
            {
                Console.WriteLine("请输入邮箱.");
                var user = Console.ReadLine();
                Console.WriteLine("请输入密码.");
                var password = Console.ReadLine();
                var api = new PixivApi(user, password);
                Console.WriteLine(@"下载此用户的作品请输入1, 要下载收藏夹请输入2, 下载搜索请输入3.");
                var mode = Console.ReadLine();
                switch (mode)
                {
                    case "1":
                        // 下载作品

                        return;
                    case "2":
                        // 下载收藏
                        Console.WriteLine("请输入下载图片数量.");
                        var countstring = Console.ReadLine();
                        var count = int.MaxValue;
                        if (countstring.IsNumber())
                        {
                            count = Int32.Parse(countstring);
                        }
                        Console.WriteLine("请输入被操作的用户id.");
                        var page = 1;
                        var uid = Console.ReadLine();
                        var favoritework = api.GetFavoriteWork(uid, count);
                        var urls = new List<string>();
                        Console.WriteLine($"已获取收藏作品,数量{favoritework.illusts.Length}");
                        foreach (var illust in favoritework.illusts)
                        {
                            if (illust.meta_pages.Length == 0)
                            {
                                urls.Add(illust.meta_single_page.original_image_url);
                            }
                            foreach (var metaPage in illust.meta_pages)
                            {
                                urls.Add(metaPage.image_urls.original);
                            }
                        }
                        Console.WriteLine($"已经获取所有图片链接,总计{urls.Count}个文件");
                        Console.WriteLine($"正在下载");
                        DownloadPictures(urls, $"{uid}_图片收藏");
                        Console.WriteLine($"下载完毕.");
            Console.ReadKey();
                        return;
                    case "3":
                        // 下载搜素

                        return;
                }
            }
        }
    }
    public static class  StringExtension
    {
        public static bool IsNumber(this string source)
        {
            return int.TryParse(source, out _);
        }
    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TRKS.github.ImageDownloader
{
    public static class WebHelper
    {
        private static ThreadLocal<WebClient> webClient = new ThreadLocal<WebClient>(() => CreateDefaultWebClient());
        private static WebClient CreateDefaultWebClient()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            webClient.Encoding = Encoding.ASCII;
            return webClient;
        }

        public static void DowloadFile(string url, string path, string name)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            webClient.Value.Headers["Referer"] = $"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={name.Split('_')[0]}";

            webClient.Value.DownloadFile(url, Path.Combine(path, name));
        }
    }
}

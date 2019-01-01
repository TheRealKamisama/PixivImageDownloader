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
        private static ThreadLocal<WebClient> webClient = new ThreadLocal<WebClient>(() => new WebClient { Encoding = Encoding.UTF8 });

        public static void DowloadFile(string url, string path, string name)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            webClient.Value.DownloadFile(url, Path.Combine(path, name));
        }
    }
}

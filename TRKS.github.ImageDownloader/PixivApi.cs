using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TRKS.github.ImageDownloader
{
    public class PixivApi
    {
        private string accessToken;

        public PixivApi(string userName, string password)
        {
            accessToken = GetAccessToken(userName, password).access_token;
        }

        private WebClient CreateDefaultWebClient()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("Referer", "http://www.pixiv.net");
            webClient.Headers.Add("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)");
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            webClient.Encoding = Encoding.ASCII;
            return webClient;
        }

        private WebClient CreateApiWebClient()
        {
            var webClient = new WebClient();
            //webClient.Headers.Add("Host", "public-api.secure.pixiv.net");
            //webClient.Headers.Add("Referer", "http://spapi.pixiv.net/");
            webClient.Headers.Add("Authorization", $"Bearer {accessToken}");
            //webClient.Headers.Add("Accept-Encoding", "gzip, deflate");
            //webClient.Headers.Add("Accept", "*/*");
            //webClient.Headers.Add("Accept-Language", "zh-cn");
            webClient.Headers.Add("App-OS-Version", "9.0.2");
            webClient.Headers.Add("App-OS", "ios");
            webClient.Headers.Add("App-Version", "6.0.4");
            // webClient.Headers.Add("Proxy-Connection", "keep-alive");
            // webClient.Headers.Add("Connection", "keep-alive");
            webClient.Headers.Add("User-Agent", "PixivIOSApp/6.0.4 (iOS 9.0.2; iPhone6,1)");
            webClient.Proxy = WebRequest.GetSystemWebProxy();
            webClient.Encoding = Encoding.Default;

            return webClient;
        }
        /*
        private static string GetPostKey() 意义不明
        {
            const string url = "https://accounts.pixiv.net/login?lang=zh&source=pc&view_type=page&ref=wwwtop_accounts_index";
            // webClient.UploadString(url, )
        }
        */
        public FavoriteWork GetFavoriteWork(string uid, int count)
        {
            var url =
                $"https://app-api.pixiv.net/v1/user/bookmarks/illust?user_id={uid}&restrict=public";
            var result = new FavoriteWork();
            var webClient = CreateApiWebClient();
            while (true)
            {
                var one_result = webClient.DownloadString(url).JsonDeserialize<FavoriteWork>();
                url = one_result.next_url;
                result.illusts.AddRange(one_result.illusts);
                if (result.illusts.Count >= count)
                {
                    result.illusts = result.illusts.Take(count).ToList();
                    break;
                }
                if (url is null)
                {
                    break;
                }
            }

            return result;
        }

        private Response GetAccessToken(string userName, string password)
        {
            const string url = "https://oauth.secure.pixiv.net/auth/token";
            var postData = $"username={userName}&password={password}&grant_type=password&client_id=bYGKuGVw91e0NMfPGp44euvGt59s&client_secret=HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";
            /* 
                postData = "get_secure_url=1&grant_type=refresh_token&client_id=bYGKuGVw91e0NMfPGp44euvGt59s&client_secret=HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK&refresh_id=" + refresh_token;
            */
            var webClient = CreateDefaultWebClient();
            var result_string = webClient.UploadString(url, postData);
            var result = result_string.JsonDeserialize<OAuthResult>();
            return result.Response;
        }

        private class OAuthResult
        {
            public Response Response { get; set; }
        }

        private class Response
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
            public string refresh_token { get; set; }
            public User user { get; set; }
            public string device_token { get; set; }
        }

        private class User
        {
            public Profile_Image_Urls profile_image_urls { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string account { get; set; }
            public string mail_address { get; set; }
            public bool is_premium { get; set; }
            public int x_restrict { get; set; }
            public bool is_mail_authorized { get; set; }
        }

        private class Profile_Image_Urls
        {
            public string px_16x16 { get; set; }
            public string px_50x50 { get; set; }
            public string px_170x170 { get; set; }
        }



    }
}

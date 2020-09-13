using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GammaLibrary.Extensions;
using Newtonsoft.Json;

namespace TRKS.github.ImageDownloader
{
    public class PixivApi
    {
        private string accessToken;
        private const string ClientHash = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
        private static string UtcTimeNow => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
        public PixivApi(string userName, string password)
        {
            accessToken = GetAccessToken(userName, password);
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

        private string GetAccessToken(string userName, string password)
        {
            if (!(Config.UserData.Instance.ExpireDate <= DateTime.Now))
            {
                return Config.UserData.Instance.AccessToken;
            }
            var time = UtcTimeNow;
            var hash = (time + ClientHash).Hash<MD5CryptoServiceProvider>();
            const string url = "https://oauth.secure.pixiv.net/auth/token";
            var postData = "get_secure_url=1&grant_type=refresh_token&client_id=bYGKuGVw91e0NMfPGp44euvGt59s&client_secret=HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK&refresh_id=" + Config.UserData.Instance.RefreshToken;
            if (Config.UserData.Instance.RefreshToken.IsNullOrEmpty())
            {
                postData = $"username={userName}&password={password}&grant_type=password&client_id=MOBrBDS8blbauoSck0ZfDbtuzpyT&client_secret=lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
            }

            var webClient = CreateDefaultWebClient();
            webClient.Headers.Add("X-Client-Time", time);
            webClient.Headers.Add("X-Client-Hash", hash);
            var result_string = webClient.UploadString(url, postData);
            var result = result_string.JsonDeserialize<OAuthResult>();
            Config.UserData.Instance.RefreshToken = result.Response.refresh_token;
            Config.UserData.Instance.AccessToken = result.Response.access_token;
            Config.UserData.Instance.ExpireDate = DateTime.Now + TimeSpan.FromSeconds(result.Response.expires_in);
            Config.UserData.Save();
            return result.Response.access_token;
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

    public static class Strings
    {
        public static string Hash<T>(this string str) where T : HashAlgorithm, new()
        {
            using (var crypt = new T())
            {
                var hashBytes = crypt.ComputeHash(str.GetBytes());
                return hashBytes.Select(b => b.ToString("x2")).Aggregate((s1, s2) => s1 + s2);
            }
        }

        public static byte[] GetBytes(this string str, Encoding encoding = null)
        {
            return encoding == null ? Encoding.UTF8.GetBytes(str) : encoding.GetBytes(str);
        }
    }
}

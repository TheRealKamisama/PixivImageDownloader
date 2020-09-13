using System;
using System.Collections.Generic;

namespace TRKS.github.ImageDownloader
{
    public class Config
    {
        [Configuration("UserData")]
        internal class UserData : Configuration<UserData>
        {
            public DateTime ExpireDate { get; set; }
            
            public string AccessToken { get; set; }

            public string RefreshToken { get; set; }
        }
    }
}
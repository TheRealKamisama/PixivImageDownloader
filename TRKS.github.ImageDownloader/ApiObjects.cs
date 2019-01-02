using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRKS.github.ImageDownloader
{
    public class ApiObjects
    {



    }
    public class FavoriteWork
    {
        public List<Illust> illusts { get; set; } = new List<Illust>();
        public string next_url { get; set; }
    }

    public class Illust
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public Image_Urls image_urls { get; set; }
        public string caption { get; set; }
        public int restrict { get; set; }
        public User user { get; set; }
        public Tag[] tags { get; set; }
        public string[] tools { get; set; }
        public DateTime create_date { get; set; }
        public int page_count { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int sanity_level { get; set; }
        public int x_restrict { get; set; }
        public object series { get; set; }
        public Meta_Single_Page meta_single_page { get; set; }
        public Meta_Pages[] meta_pages { get; set; }
        public int total_view { get; set; }
        public int total_bookmarks { get; set; }
        public bool is_bookmarked { get; set; }
        public bool visible { get; set; }
        public bool is_muted { get; set; }
    }

    public class Image_Urls
    {
        public string square_medium { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string account { get; set; }
        public Profile_Image_Urls profile_image_urls { get; set; }
        public bool is_followed { get; set; }
    }

    public class Profile_Image_Urls
    {
        public string medium { get; set; }
    }

    public class Meta_Single_Page
    {
        public string original_image_url { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
    }

    public class Meta_Pages
    {
        public Image_Urls1 image_urls { get; set; }
    }

    public class Image_Urls1
    {
        public string square_medium { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string original { get; set; }
    }
}

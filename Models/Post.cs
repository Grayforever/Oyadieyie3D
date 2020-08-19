namespace Oyadieyie3D.Models
{
    public class Post
    {
        public string Author { get; set; }
        public string ImageId { get; set; }
        public string DownloadUrl { get; set; }
        public string PostBody { get; set; }
        public int LikeCount { get; set; }
        public bool Liked { get; set; }
        public string ID { get; set; }
        public string OwnerId { get; set; }
        public string PostDate { get; set; }

    }
}
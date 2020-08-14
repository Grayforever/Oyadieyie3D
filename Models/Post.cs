using Android.OS;
using Android.Runtime;
using Java.Interop;

namespace Oyadieyie3D.Models
{
    public class Post : Java.Lang.Object, IParcelable
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

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Author);
            dest.WriteString(ImageId);
            dest.WriteString(DownloadUrl);
            dest.WriteString(PostBody);
            dest.WriteString(ID);
            dest.WriteString(OwnerId);
            dest.WriteString(PostDate);

            dest.WriteInt(LikeCount);
            dest.WriteBoolean(Liked);
        }

        private Post(Parcel inn)
        {
            Author = inn.ReadString();
            ImageId = inn.ReadString();
            DownloadUrl = inn.ReadString();
            PostBody = inn.ReadString();
            ID = inn.ReadString();
            OwnerId = inn.ReadString();
            PostDate = inn.ReadString();

            LikeCount = inn.ReadInt();
            Liked = inn.ReadBoolean();
        }

        public Post()
        {

        }

        private static readonly GenericParcelableCreator<Post> _creator
            = new GenericParcelableCreator<Post>((parcel) => new Post(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<Post> GetCreator()
        {
            return _creator;
        }
    }
}
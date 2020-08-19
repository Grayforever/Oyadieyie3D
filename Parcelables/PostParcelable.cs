using Android.OS;
using Android.Runtime;
using Java.Interop;
using Oyadieyie3D.Models;

namespace Oyadieyie3D.Parcelables
{
    public class PostParcelable : Java.Lang.Object, IParcelable
    {
        public Post PostItem { get; set; }
        private PostParcelable(Parcel parcel)
        {
            PostItem = new Post()
            {
                Author = parcel.ReadString(),
                ImageId = parcel.ReadString(),
                DownloadUrl = parcel.ReadString(),
                PostBody = parcel.ReadString(),
                ID = parcel.ReadString(),
                OwnerId = parcel.ReadString(),
                PostDate = parcel.ReadString(),

                LikeCount = parcel.ReadInt(),
                Liked = parcel.ReadBoolean()
            };
        }

        public PostParcelable()
        {

        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(PostItem.Author);
            dest.WriteString(PostItem.ImageId);
            dest.WriteString(PostItem.DownloadUrl);
            dest.WriteString(PostItem.PostBody);
            dest.WriteString(PostItem.ID);
            dest.WriteString(PostItem.OwnerId);
            dest.WriteString(PostItem.PostDate);

            dest.WriteInt(PostItem.LikeCount);
            dest.WriteBoolean(PostItem.Liked);
        }

        private static readonly GenericParcelableCreator<PostParcelable> _creator
            = new GenericParcelableCreator<PostParcelable>((parcel) => new PostParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<PostParcelable> GetCreator()
        {
            return _creator;
        }
    }
}
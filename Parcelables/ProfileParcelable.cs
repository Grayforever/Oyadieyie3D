using Android.OS;
using Android.Runtime;
using Java.Interop;
using Oyadieyie3D.Models;
using System;

namespace Oyadieyie3D.Parcelables
{
    internal sealed class ProfileParcelable : Java.Lang.Object, IParcelable
    {
        public User UserProfile { get; set; }
        public ProfileParcelable()
        {

        }

        public ProfileParcelable(Parcel parcel)
        {
            UserProfile = new User
            {
                ID = parcel.ReadString(),
                Username = parcel.ReadString(),
                ProfileImgUrl = parcel.ReadString(),
                Status = parcel.ReadString(),
                Email = parcel.ReadString(),
                Phone = parcel.ReadString()
            };
        }
        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            try
            {
                dest.WriteString(UserProfile.ID);
                dest.WriteString(UserProfile.Username);
                dest.WriteString(UserProfile.ProfileImgUrl);
                dest.WriteString(UserProfile.Status);
                dest.WriteString(UserProfile.Email);
                dest.WriteString(UserProfile.Phone);
            }
            catch (Exception)
            {
                
            }
        }

        private static readonly GenericParcelableCreator<ProfileParcelable> _creator
            = new GenericParcelableCreator<ProfileParcelable>((parcel) => new ProfileParcelable(parcel));

        [ExportField("CREATOR")]
        public static GenericParcelableCreator<ProfileParcelable> GetCreator()
        {
            return _creator;
        }
    }
}
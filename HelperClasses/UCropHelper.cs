using Android.Content;
using Android.Database;
using Android.Provider;
using AndroidX.Core.Content;
using Com.Yalantis.Ucrop;
using Java.IO;
using Java.Lang;
using static AndroidX.Core.Content.FileProvider;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.HelperClasses
{
    public class UCropHelper
    {
        private Context _context;
        private static UCropHelper helper;
        public UCropHelper(Context context)
        {
            _context = context;
        }

        public static void Init(Context context)
        {
            if(context != null)
            {
                if(helper == null)
                {
                    lock (typeof(UCropHelper))
                    {
                        if (helper == null)
                        {
                            helper = new UCropHelper(context);
                        }
                    }
                }
            }
            
        }

        public static UCropHelper Instance => helper switch
        {
            null => throw new IllegalStateException("helper is not initialized, call init"),
            _ => helper
        };

        public void CropImage(Uri selectedImageURI, MainActivity activity)
        {
            Uri destinationUri = Uri.FromFile(new File(_context.CacheDir, QueryName(_context.ContentResolver, selectedImageURI)));
            UCrop.Options options = new UCrop.Options();
            options.SetCompressionQuality(Constants.IMAGE_COMPRESSION);
            options.SetToolbarColor(ContextCompat.GetColor(_context, Resource.Color.colorPrimary));
            options.SetStatusBarColor(ContextCompat.GetColor(_context, Resource.Color.colorPrimaryDark));
            options.SetActiveControlsWidgetColor(ContextCompat.GetColor(_context, Resource.Color.colorAccent));

            if (Constants.lockAspectRatio)
                options.WithAspectRatio(Constants.ASPECT_RATIO_X, Constants.ASPECT_RATIO_Y);

            if (Constants.setBitmapMaxWidthHeight)
                options.WithMaxResultSize(Constants.bitmapMaxWidth, Constants.bitmapMaxHeight);

            UCrop.Of(selectedImageURI, destinationUri)
                .WithOptions(options)
                .Start(activity, UCrop.RequestCrop);
        }

        public static string QueryName(ContentResolver contentResolver, Uri uri)
        {
            ICursor returnCursor = contentResolver.Query(uri, null, null, null, null);
            System.Diagnostics.Debug.Assert(returnCursor != null);
            int nameIndex = returnCursor.GetColumnIndex(OpenableColumns.DisplayName);
            returnCursor.MoveToFirst();
            string name = returnCursor.GetString(nameIndex);
            returnCursor.Close();
            return name;
        }

        public Uri GetCacheImagePath(string fileName)
        {
            File path = new File(_context.ExternalCacheDir, "camera");
            if (!path.Exists()) path.Mkdirs();
            File image = new File(path, fileName);
            return GetUriForFile(_context, _context.PackageName + ".fileprovider", image);
        }

        public void ClearCache()
        {
            File path = new File(_context.ExternalCacheDir, "camera");
            if (path.Exists() && path.IsDirectory)
            {
                foreach (var child in path.ListFiles())
                {
                    child.Delete();
                }
            }
        }
    }

}
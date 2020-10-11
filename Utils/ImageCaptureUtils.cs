using Android;
using Android.Content;
using Android.Database;
using Android.Provider;
using Java.IO;
using Karumi.DexterLib;
using Oyadieyie3D.Events;
using System;
using static AndroidX.Core.Content.FileProvider;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Utils
{
    public class ImageCaptureUtils
    {
        private Context _context;
        private string fileName;

        public event EventHandler<ImageSelectedEventArgs> OnImageCaptured;
        public event EventHandler<ImageSelectedEventArgs> OnImageSelected;


        public class ImageSelectedEventArgs : EventArgs
        {
            public Intent imageIntent { get; set; }
            public string fileName { get; set; }
        }

        public ImageCaptureUtils(Context context)
        {
            _context = context;
        }
        public void FetchImageFromGallery()
        {
            Dexter.WithContext(_context)
                .WithPermissions(Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage)
                .WithListener(new MultiplePermissionsListener((permissions, token) =>
                {
                    token.ContinuePermissionRequest();
                }, (report) =>
                {
                    if (report.AreAllPermissionsGranted())
                    {
                        Intent intent = new Intent();
                        intent.SetType("image/*");
                        intent.SetAction(Intent.ActionGetContent);
                        OnImageSelected?.Invoke(this, new ImageSelectedEventArgs { imageIntent = intent });
                    }
                })).Check();
        }

        public void TakeCameraImage()
        {
            Dexter.WithContext(_context)
                .WithPermissions(Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage)
                .WithListener(new MultiplePermissionsListener((permissions, token) =>
                {
                    token.ContinuePermissionRequest();
                }, (report) =>
                {
                    if (report.AreAllPermissionsGranted())
                    {
                        fileName = GenerateRandomString(6) + ".jpg";
                        Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
                        takePictureIntent.PutExtra(MediaStore.ExtraOutput, GetCacheImagePath(fileName));
                        if (takePictureIntent.ResolveActivity(_context.PackageManager) != null)
                        {
                            OnImageCaptured?.Invoke(this, new ImageSelectedEventArgs { imageIntent = takePictureIntent, fileName = fileName});
                        }
                    }
                })).Check();

        }

        private static string QueryName(ContentResolver contentResolver, Uri uri)
        {
            ICursor returnCursor = contentResolver.Query(uri, null, null, null, null);
            System.Diagnostics.Debug.Assert(returnCursor != null);
            int nameIndex = returnCursor.GetColumnIndex(OpenableColumns.DisplayName);
            returnCursor.MoveToFirst();
            string name = returnCursor.GetString(nameIndex);
            returnCursor.Close();
            return name;
        }


        private Uri GetCacheImagePath(string fileName)
        {
            File path = new File(_context.ExternalCacheDir, "camera");
            if (!path.Exists()) path.Mkdirs();
            File image = new File(path, fileName);
            return GetUriForFile(_context, _context.PackageName + ".fileprovider", image);
        }

        private string GenerateRandomString(int lenght)
        {
            Random rand = new Random();
            char[] allowchars = "ABCDEFGHIJKLOMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            string sResult = "";
            for (int i = 0; i <= lenght; i++)
            {
                sResult += allowchars[rand.Next(0, allowchars.Length)];
            }

            return sResult;
        }

        public void ClearCache()
        {
            File path = new File(_context.ExternalCacheDir, "camera");
            if (path.Exists() && path.IsDirectory)
            {
                foreach (File child in path.ListFiles())
                {
                    child.Delete();
                }
            }
        }


    }
}
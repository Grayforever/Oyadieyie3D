using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Views;
using AndroidX.Core.Content;
using Com.Yalantis.Ucrop;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.BottomSheet;
using Java.IO;
using Oyadieyie3D.Utils;
using System;
using static AndroidX.Core.Content.FileProvider;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Fragments
{
    public class ProfileChooserFragment : BottomSheetDialogFragment
    {
        private const int SELECT_PICTURE = 500;
        private int REQUEST_IMAGE_CAPTURE = 600;
        private ImageCaptureUtils icu;
        
        private string fileName;

        private bool lockAspectRatio = false, setBitmapMaxWidthHeight = false;
        private int ASPECT_RATIO_X = 16, ASPECT_RATIO_Y = 9, bitmapMaxWidth = 1000, bitmapMaxHeight = 1000;
        private int IMAGE_COMPRESSION = 80;

        public event EventHandler<CropCompleteEventArgs> OnCropComplete;
        public class CropCompleteEventArgs : EventArgs
        {
            public Uri imageUri { get; set; }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            icu = new ImageCaptureUtils(Context);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.change_profile_bottomsheet, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var camBtn = view.FindViewById<CircleImageView>(Resource.Id.camera_btn);
            var galBtn = view.FindViewById<CircleImageView>(Resource.Id.gallery_btn);
            var remBtn = view.FindViewById<CircleImageView>(Resource.Id.remove_btn);
            
            camBtn.Click += CamBtn_Click;
            galBtn.Click += GalBtn_Click;
            remBtn.Click += RemBtn_Click;
        }

        private void RemBtn_Click(object sender, System.EventArgs e)
        {
            
        }

        private void GalBtn_Click(object sender, System.EventArgs e)
        {
            
            icu.FetchImageFromGallery();
            icu.OnImageSelected += Icu_OnImageSelected;
        }

        private void Icu_OnImageSelected(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(Intent.CreateChooser(e.imageIntent, "Select Picture"), SELECT_PICTURE);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (resultCode == (int)Result.Ok)
                {
                    if (requestCode == SELECT_PICTURE)
                    {
                        Uri selectedImageURI = data.Data;
                        CropImage(selectedImageURI);
                    }
                    else if (requestCode == REQUEST_IMAGE_CAPTURE)
                    {
                        CropImage(GetCacheImagePath(fileName));
                    }
                    else if (requestCode == UCrop.RequestCrop)
                    {
                        var uri = UCrop.GetOutput(data);
                        OnCropComplete?.Invoke(this, new CropCompleteEventArgs { imageUri = uri });
                        DismissAllowingStateLoss();
                    }
                    else if (requestCode == UCrop.ResultError)
                    {
                        throw UCrop.GetError(data);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void CropImage(Uri selectedImageURI)
        {
            Uri destinationUri = Uri.FromFile(new File(Context.CacheDir, QueryName(Context.ContentResolver, selectedImageURI)));
            UCrop.Options options = new UCrop.Options();
            options.SetCompressionQuality(IMAGE_COMPRESSION);
            options.SetToolbarColor(ContextCompat.GetColor(Context, Resource.Color.colorPrimary));
            options.SetStatusBarColor(ContextCompat.GetColor(Context, Resource.Color.colorPrimaryDark));
            options.SetActiveControlsWidgetColor(ContextCompat.GetColor(Context, Resource.Color.colorAccent));

            if (lockAspectRatio)
                options.WithAspectRatio(ASPECT_RATIO_X, ASPECT_RATIO_Y);

            if (setBitmapMaxWidthHeight)
                options.WithMaxResultSize(bitmapMaxWidth, bitmapMaxHeight);

            UCrop.Of(selectedImageURI, destinationUri)
                .WithOptions(options)
                .Start(Context, this, UCrop.RequestCrop);
        }

        private Uri GetCacheImagePath(string fileName)
        {
            File path = new File(Activity.ExternalCacheDir, "camera");
            if (!path.Exists()) path.Mkdirs();
            File image = new File(path, fileName);
            return GetUriForFile(Context, Context.PackageName + ".fileprovider", image);
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

        private void CamBtn_Click(object sender, System.EventArgs e)
        {
            icu.TakeCameraImage();
            icu.OnImageCaptured += Icu_OnImageCaptured;
        }

        private void Icu_OnImageCaptured(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(e.imageIntent, REQUEST_IMAGE_CAPTURE);
            fileName = e.fileName;
        }
    }
}
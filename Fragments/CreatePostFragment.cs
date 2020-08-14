using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using BumpTech.GlideLib;
using Com.Yalantis.Ucrop;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Java.IO;
using Karumi.DexterLib;
using Oyadieyie3D.Events;
using System;
using static AndroidX.Core.Content.FileProvider;
using static Oyadieyie3D.MainActivity;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Fragments
{
    public class CreatePostFragment : BottomSheetDialogFragment
    {
        private const int UCROP_REQUEST = 69;
        private static int SELECT_PICTURE = 1;
        private ImageView postImageView;

        public   int REQUEST_IMAGE_CAPTURE = 500;
        private bool lockAspectRatio = false, setBitmapMaxWidthHeight = false;
        private int ASPECT_RATIO_X = 16, ASPECT_RATIO_Y = 9, bitmapMaxWidth = 1000, bitmapMaxHeight = 1000;
        private int IMAGE_COMPRESSION = 80;
        public static string fileName;

        public override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.create_post_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var appbar = view.FindViewById<AppBarLayout>(Resource.Id.create_post_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            var postToggleBtn = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.photo_choser_togglegrp);
            postImageView = view.FindViewById<ImageView>(Resource.Id.image_to_post);

            toolbar.Title = "Create Post";
            toolbar.InflateMenu(Resource.Menu.menu_create_post);
            var saveBtn = toolbar.Menu.FindItem(Resource.Id.action_save);

            toolbar.SetOnMenuItemClickListener(new OnMenuItemClickListener((menuItem)=> 
            {
                switch (menuItem.ItemId)
                {
                    case Resource.Id.action_save:
                        break;

                    case Resource.Id.action_close:
                        ClearCache(Context);
                        DismissAllowingStateLoss();
                        break;
                }
            }, false));
            RegisterForContextMenu(postImageView);
            postToggleBtn.AddOnButtonCheckedListener(new ButtonCheckedListener((g, id, isChecked) =>
            {
            if (isChecked)
            {
                switch (id)
                {
                    case Resource.Id.open_cam_btn:
                            TakeCameraImage();
                            break;
                        case Resource.Id.choose_photo_btn:
                            FetchImageFromGallery();
                            break;
                    }
                }
            }));

            
        }

        private void TakeCameraImage()
        {
            Dexter.WithContext(Context)
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
                        if (takePictureIntent.ResolveActivity(Context.PackageManager) != null)
                        {
                            StartActivityForResult(takePictureIntent, REQUEST_IMAGE_CAPTURE);
                        }
                    }
                })).Check();
            
        }

        private void FetchImageFromGallery()
        {
            Dexter.WithContext(Context)
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
                        StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), SELECT_PICTURE);
                    }
                })).Check();
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
                    else if(requestCode == REQUEST_IMAGE_CAPTURE)
                    {
                        CropImage(GetCacheImagePath(fileName));
                    }
                    else if (requestCode == UCROP_REQUEST)
                    {
                        var uri = UCrop.GetOutput(data);
                        Glide.With(this).Load(uri).Into(postImageView);
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
                .Start(Context, this, UCROP_REQUEST);
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
            File path = new File(Activity.ExternalCacheDir, "camera");
            if (!path.Exists()) path.Mkdirs();
            File image = new File(path, fileName);
            return GetUriForFile(Context, Context.PackageName + ".fileprovider", image);
        }

        private string GenerateRandomString(int lenght)
        {
            System.Random rand = new Random();
            char[] allowchars = "ABCDEFGHIJKLOMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            string sResult = "";
            for (int i = 0; i <= lenght; i++)
            {
                sResult += allowchars[rand.Next(0, allowchars.Length)];
            }

            return sResult;
        }

        public void ClearCache(Context context)
        {
            File path = new File(Context.ExternalCacheDir, "camera");
            if (path.Exists() && path.IsDirectory)
            {
                foreach (File child in path.ListFiles())
                {
                    child.Delete();
                }
            }
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            menu.SetHeaderTitle("Photo Option");
            menu.Add(0, v.Id, 0, "Remove");
            menu.Add(0, v.Id, 0, "Crop");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            Toast.MakeText(Context, item.ItemId + "", ToastLength.Short).Show();
            return base.OnContextItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
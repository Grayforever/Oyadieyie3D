using Android.App;
using Android.Content;
using Android.Database;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using Com.Yalantis.Ucrop;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Java.IO;
using Java.Util;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Utils;
using System;
using static AndroidX.Core.Content.FileProvider;
using Task = Android.Gms.Tasks.Task;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Fragments
{
    public class CreatePostFragment : BottomSheetDialogFragment
    {
        private const int UCROP_REQUEST = 69;
        private const string HAS_IMAGE_KEY = "has_image";
        private static int SELECT_PICTURE = 1;
        private ImageView postImageView;
        private CircleImageView profile_image_view;
        private TextInputLayout commentEt;
        private MaterialButton doneBtn;
        public   int REQUEST_IMAGE_CAPTURE = 500;
        private bool lockAspectRatio = false, setBitmapMaxWidthHeight = false;
        private int ASPECT_RATIO_X = 16, ASPECT_RATIO_Y = 9, bitmapMaxWidth = 1000, bitmapMaxHeight = 1000;
        private int IMAGE_COMPRESSION = 80;
        public static string fileName;
        private string profile_url;
        private ImageCaptureUtils icu;
        private bool hasImage = false;
        private ProgressBar postProgress;
        private static StorageReference imageRef;
        private ImageButton closeBtn;
        private Uri uri;
        private RequestOptions requestOptions;

        public event EventHandler OnPostComplete;
        public event EventHandler<ErrorEventArgs> OnErrorEncounted;
        public class ErrorEventArgs : EventArgs
        {
            public string ErrorMsg { get; set; }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            profile_url = Arguments.GetString(Constants.IMG_URL_KEY);
            icu = new ImageCaptureUtils(Context);
            icu.OnImageCaptured += Icu_OnImageCaptured;
            icu.OnImageSelected += Icu_OnImageSelected;

            requestOptions = new RequestOptions();
            requestOptions.Placeholder(Resource.Drawable.img_placeholder);
            requestOptions.SkipMemoryCache(true);
        }

        public override int Theme => Resource.Style.BottomSheetDialog_Rounded;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.create_post_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var postToggleBtn = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.photo_choser_togglegrp);
            postImageView = view.FindViewById<ImageView>(Resource.Id.image_to_post);
            profile_image_view = view.FindViewById<CircleImageView>(Resource.Id.create_post_userProfile);
            commentEt = view.FindViewById<TextInputLayout>(Resource.Id.create_post_et);
            doneBtn = view.FindViewById<MaterialButton>(Resource.Id.done_btn);
            postProgress = view.FindViewById<ProgressBar>(Resource.Id.post_progress);
            closeBtn = view.FindViewById<ImageButton>(Resource.Id.post_cancel_btn);
            closeBtn.Click += CloseBtn_Click;
            postToggleBtn.AddOnButtonCheckedListener(new ButtonCheckedListener(
            onbuttonChecked: (g, id, isChecked) =>
            {
            if (isChecked)
            {
                switch (id)
                {
                    case Resource.Id.open_cam_btn:
                            icu.TakeCameraImage();
                            break;

                        case Resource.Id.choose_photo_btn:
                            icu.FetchImageFromGallery();
                            break;
                    }
                }
            }));

            doneBtn.Click += DoneBtn_Click;
            commentEt.EditText.TextChanged += EditText_TextChanged;
            Glide.With(this).SetDefaultRequestOptions(requestOptions).Load(profile_url).Into(profile_image_view);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            DismissAllowingStateLoss();
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            doneBtn.Post(async () =>
            {
                postProgress.Visibility = ViewStates.Visible;
                doneBtn.Enabled = false;
                doneBtn.Text = "Posting";
                try
                {
                    var stream = new System.IO.MemoryStream();
                    var bitmap = MediaStore.Images.Media.GetBitmap(Context.ContentResolver, uri);
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Webp, 70, stream);
                    var imgArray = stream.ToArray();

                    var postRef = SessionManager.GetFireDB().GetReference("posts").Push();
                    string imageId = postRef.Key;
                    imageRef = FirebaseStorage.Instance.GetReference("postImages/" + imageId);
                    imageRef.PutBytes(imgArray).ContinueWithTask(new ContinuationTask(
                    then: t =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                    })).AddOnCompleteListener(new OncompleteListener(
                    onComplete: t =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                        var postMap = new HashMap();
                        postMap.Put("owner_id", SessionManager.GetFirebaseAuth().CurrentUser.Uid);
                        postMap.Put("post_date", DateTime.UtcNow.ToString());
                        postMap.Put("post_body", commentEt.EditText.Text);
                        postMap.Put("download_url", t.Result.ToString());
                        postMap.Put("image_id", imageId);
                        postMap.Put("post_date", DateTime.UtcNow.ToString());
                        postRef.SetValue(postMap).AddOnCompleteListener(new OncompleteListener(
                        onComplete: task =>
                        {
                            try
                            {
                                if (!task.IsSuccessful)
                                    throw task.Exception;

                                DismissAllowingStateLoss();
                                OnPostComplete?.Invoke(this, new EventArgs());
                            }
                            catch (DatabaseException de)
                            {
                                DismissAllowingStateLoss();
                                OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = de.Message});
                            }

                        }));
                        postRef.KeepSynced(true);

                    }));
                    
                }
                catch (DatabaseException fde)
                {
                    DismissAllowingStateLoss();
                    OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = fde.Message });
                }
                catch (FirebaseNetworkException fne)
                {
                    DismissAllowingStateLoss();
                    OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = fne.Message });
                }
                catch (StorageException se)
                {
                    DismissAllowingStateLoss();
                    OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = se.Message });
                }
                catch (FirebaseAuthException fae)
                {
                    OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = fae.Message });
                    DismissAllowingStateLoss();
                }
                catch (Exception ex)
                {
                    DismissAllowingStateLoss();
                    OnErrorEncounted?.Invoke(this, new ErrorEventArgs { ErrorMsg = ex.Message });
                }
            });
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
        }

        private void Icu_OnImageSelected(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(Intent.CreateChooser(e.imageIntent, "Select Picture"), SELECT_PICTURE);
        }

        private void Icu_OnImageCaptured(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(e.imageIntent, REQUEST_IMAGE_CAPTURE);
            fileName = e.fileName;
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
                    else if (requestCode == UCrop.RequestCrop)
                    {
                        uri = UCrop.GetOutput(data);
                        Glide.With(this).Load(uri).Into(postImageView);
                        hasImage = true;
                        doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
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

        public void ClearCache()
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

        internal sealed class ContinuationTask : Java.Lang.Object, IContinuation
        {
            private Action<Task> _then;

            public ContinuationTask(Action<Task> then)
            {
                _then = then;
            }

            public Java.Lang.Object Then(Task task)
            {
                _then?.Invoke(task);
                return imageRef.GetDownloadUrl();
            }
        }
    }
}